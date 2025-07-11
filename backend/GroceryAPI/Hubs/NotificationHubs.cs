using Microsoft.AspNetCore.SignalR;

namespace GroceryAPI.Hubs
{
    public class NotificationHubs : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
