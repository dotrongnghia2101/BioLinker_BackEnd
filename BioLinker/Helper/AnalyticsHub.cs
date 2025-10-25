using Microsoft.AspNetCore.SignalR;

namespace BioLinker.Helper
{
    public class AnalyticsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Có thể dùng để log user khi connect
            await base.OnConnectedAsync();
        }

        // Client sẽ gọi API này để join group của 1 static link
        public async Task JoinStaticLinkGroup(string staticLinkId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"static_{staticLinkId}");
        }

        public async Task LeaveStaticLinkGroup(string staticLinkId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"static_{staticLinkId}");
        }
    }
}
