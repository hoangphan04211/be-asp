using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "products");
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}
