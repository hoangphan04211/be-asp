using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using QLKHO_PhanVanHoang.Data;
using QLKHO_PhanVanHoang.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IForecastingService
    {
        ProductForecastResponseDto PredictDemand(int productId, int horizon = 30);
    }

    public class ForecastingService : IForecastingService
    {
        private readonly ApplicationDbContext _context;

        public ForecastingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public class TimeSeriesData
        {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public class ForecastPrediction
        {
            public float[] Forecast { get; set; }
            public float[] LowerBound { get; set; }
            public float[] UpperBound { get; set; }
        }

        public ProductForecastResponseDto PredictDemand(int productId, int horizon = 30)
        {
            var product = _context.Products.Find(productId);
            if (product == null) throw new Exception("Không tìm thấy sản phẩm");

            // Lấy lịch sử xuất kho (ChangeQuantity < 0)
            var transactions = _context.StockCards
                .Where(s => s.ProductId == productId && !s.IsDeleted && s.ChangeQuantity < 0)
                .Select(s => new { s.TransactionDate, Consumption = Math.Abs((float)s.ChangeQuantity) })
                .ToList();

            if (!transactions.Any())
            {
                throw new Exception("Sản phẩm chưa có dữ liệu xuất kho để dự báo.");
            }

            // Nhóm theo ngày
            var dailyData = transactions
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new { Date = g.Key, Value = g.Sum(x => x.Consumption) })
                .OrderBy(x => x.Date)
                .ToList();

            if (dailyData.Count < 7)
            {
                throw new Exception("Cần ít nhất 7 ngày có giao dịch xuất kho để chạy thuật toán dự báo (SSA). Vui lòng thêm dữ liệu lịch sử.");
            }

            // Điền các ngày trống (0) để tạo Time Series liên tục
            var startDate = dailyData.First().Date;
            var endDate = dailyData.Last().Date;
            var totalDays = (int)(endDate - startDate).TotalDays + 1;

            var timeSeries = new List<TimeSeriesData>();
            for (int i = 0; i < totalDays; i++)
            {
                var currentDate = startDate.AddDays(i);
                var existing = dailyData.FirstOrDefault(d => d.Date == currentDate);
                timeSeries.Add(new TimeSeriesData
                {
                    Date = currentDate,
                    Value = existing != null ? existing.Value : 0f
                });
            }

            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(timeSeries);

            // SSA Parameters
            var windowSize = 7; // Chu kỳ tuần
            if (timeSeries.Count >= 30) windowSize = 14; 
            var seriesLength = timeSeries.Count;

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ForecastPrediction.Forecast),
                inputColumnName: nameof(TimeSeriesData.Value),
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: seriesLength,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(ForecastPrediction.LowerBound),
                confidenceUpperBoundColumn: nameof(ForecastPrediction.UpperBound));

            var model = forecastingPipeline.Fit(dataView);

            var forecastEngine = model.CreateTimeSeriesEngine<TimeSeriesData, ForecastPrediction>(mlContext);
            var forecast = forecastEngine.Predict();

            var result = new ProductForecastResponseDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                SkuCode = product.SkuCode,
                Unit = product.Unit,
                HistoricalData = timeSeries.Select(t => new ForecastResultDto
                {
                    Date = t.Date,
                    ForecastedQuantity = t.Value,
                    LowerBound = t.Value,
                    UpperBound = t.Value
                }).ToList(),
                ForecastData = new List<ForecastResultDto>()
            };

            for (int i = 0; i < horizon; i++)
            {
                float predictedValue = forecast.Forecast[i] < 0 ? 0 : (float)Math.Round(forecast.Forecast[i], 2); // Không dự báo âm
                float lower = forecast.LowerBound[i] < 0 ? 0 : (float)Math.Round(forecast.LowerBound[i], 2);
                float upper = forecast.UpperBound[i] < 0 ? 0 : (float)Math.Round(forecast.UpperBound[i], 2);

                result.ForecastData.Add(new ForecastResultDto
                {
                    Date = endDate.AddDays(i + 1),
                    ForecastedQuantity = predictedValue,
                    LowerBound = lower,
                    UpperBound = upper
                });
            }

            return result;
        }
    }
}
