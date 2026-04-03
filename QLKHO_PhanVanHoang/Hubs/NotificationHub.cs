using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Hubs
{
    // Hub phục vụ việc gửi nhận thông báo thời gian thực
    public class NotificationHub : Hub
    {
        // Khi một client kết nối, ta có thể log hoặc gán vào Group tương ứng
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Gửi thông báo tới tất cả client đang kết nối
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Gửi thông báo tới một phòng (group) cụ thể (vd: group admin)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
