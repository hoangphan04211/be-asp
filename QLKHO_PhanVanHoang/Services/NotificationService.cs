using Microsoft.AspNetCore.SignalR;
using QLKHO_PhanVanHoang.Hubs;
using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface INotificationService
    {
        Task SendNotificationToAllAsync(string title, string message);
        Task SendNotificationToUserAsync(string userId, string title, string message);
        Task SendNotificationToRoleAsync(string roleName, string title, string message);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToAllAsync(string title, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { Title = title, Message = message });
        }

        public async Task SendNotificationToUserAsync(string userId, string title, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new { Title = title, Message = message });
        }

        public async Task SendNotificationToRoleAsync(string roleName, string title, string message)
        {
            // Giả định người dùng trong Role đã JoinGroup với tên Role đó khi kết nối
            await _hubContext.Clients.Group(roleName).SendAsync("ReceiveNotification", new { Title = title, Message = message });
        }
    }
}
