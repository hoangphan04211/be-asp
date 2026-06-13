using System;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class ForecastResultDto
    {
        public DateTime Date { get; set; }
        public float ForecastedQuantity { get; set; }
        public float LowerBound { get; set; }
        public float UpperBound { get; set; }
    }
    
    public class ProductForecastResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SkuCode { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public List<ForecastResultDto> HistoricalData { get; set; } = new List<ForecastResultDto>();
        public List<ForecastResultDto> ForecastData { get; set; } = new List<ForecastResultDto>();
    }
}
