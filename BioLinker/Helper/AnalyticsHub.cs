using Microsoft.AspNetCore.SignalR;

namespace BioLinker.Helper
{
    public class AnalyticsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string? bioPageId = Context.GetHttpContext()?.Request.Query["bioPageId"];
            if (!string.IsNullOrEmpty(bioPageId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"bio_{bioPageId}");
            }
            await base.OnConnectedAsync();
        }
    }
}
