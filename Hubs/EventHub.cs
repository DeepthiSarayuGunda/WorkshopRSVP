using Microsoft.AspNetCore.SignalR;

namespace WorkshopRSVP.Hubs
{
    public class EventHub : Hub
    {
        // Client calls this to join the group for a specific event
        public async Task JoinEventGroup(string eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event-{eventId}");
        }

        // Client calls this to leave the group for a specific event
        public async Task LeaveEventGroup(string eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event-{eventId}");
        }
    }
}
