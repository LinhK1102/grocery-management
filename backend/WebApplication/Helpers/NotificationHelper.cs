using Microsoft.AspNetCore.SignalR;
using Utility.Hubs;

namespace GroceryWebApp.Helpers
{
    public static class NotificationHelper
    {
        public static async Task NotifyAsync(IHubContext<NotificationHub> hub, bool success, string successMsg, string failMsg)
        {
            string message = success ? successMsg : failMsg;
            await hub.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }

}
