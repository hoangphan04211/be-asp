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
        Task SendForceLogoutToSessionAsync(string sessionId);
        Task SendPermissionsUpdatedToUserAsync(string userId);
        Task SendPermissionsUpdatedToRoleAsync(string roleName);
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
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message);
        }

        public async Task SendNotificationToUserAsync(string userId, string title, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, message);
        }

        public async Task SendNotificationToRoleAsync(string roleName, string title, string message)
        {
            // Giả định người dùng trong Role đã JoinGroup với tên Role đó khi kết nối
            await _hubContext.Clients.Group(roleName).SendAsync("ReceiveNotification", title, message);
        }

        public async Task SendForceLogoutToSessionAsync(string sessionId)
        {
            await _hubContext.Clients.Group($"Session_{sessionId}").SendAsync("ForceLogout");
        }

        public async Task SendPermissionsUpdatedToUserAsync(string userId)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("PermissionsUpdated");
        }

        public async Task SendPermissionsUpdatedToRoleAsync(string roleName)
        {
            await _hubContext.Clients.Group($"Role_{roleName}").SendAsync("PermissionsUpdated");
        }
    }
}
