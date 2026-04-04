using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using QLKHO_PhanVanHoang.Helpers;

namespace QLKHO_PhanVanHoang.Services
{
    public class CloudinaryService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "products")
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"WMS_{folder}",
                    // Tối ưu hóa ảnh tự động: Giảm dung lượng, giữ chất lượng (q_auto), định dạng nén (f_auto)
                    // Giới hạn chiều rộng 800px để tránh ảnh quá nặng
                    Transformation = new Transformation()
                        .Width(800).Height(800).Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUrl.ToString();
            }

            return string.Empty;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return true;

            try
            {
                // Parse PublicId from URL
                // Example: https://res.cloudinary.com/cloudname/image/upload/v1234567/WMS_products/id.jpg
                var uri = new Uri(imageUrl);
                var fileName = Path.GetFileNameWithoutExtension(uri.LocalPath);
                var segments = uri.Segments;
                
                // Cloudinary PublicId includes folder
                // Usually it's the last part or includes folders after /upload/
                string publicId = "";
                bool startCapture = false;
                foreach(var segment in segments)
                {
                    var cleanSegment = segment.Trim('/');
                    if (startCapture)
                    {
                        publicId += (string.IsNullOrEmpty(publicId) ? "" : "/") + cleanSegment;
                    }
                    if (cleanSegment == "upload") startCapture = true;
                }
                
                // Remove versioning (v1234567) if present
                if (publicId.Contains("/v") && publicId.Split('/').Length > 2)
                {
                   var parts = publicId.Split('/');
                   publicId = string.Join("/", parts, 2, parts.Length - 2);
                }

                // Remove extension again just in case
                if (publicId.Contains(".")) publicId = publicId.Split('.')[0];

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}
