using BusinessObjects.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Events
{
    public interface INotificationService
    {
        Task NotifyAsync(string eventName, object data);
    }
}
