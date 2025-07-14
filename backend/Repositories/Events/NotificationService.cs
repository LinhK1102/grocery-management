using BusinessObjects.Events;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Hubs;

namespace Repositories.Events
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            //if (domainEvent is OrderCreatedEvent evt)
            //{
            //    await _hubContext.Clients.All.SendAsync("OrderCreated", new
            //    {
            //        evt.OrderId,
            //        evt.ProductName
            //    });
            //}

            // Add more event types here if needed
            await Task.CompletedTask;
        }

        public Task NotifyAsync(string eventName, object data)
        {
            throw new NotImplementedException();
        }
    }
}
