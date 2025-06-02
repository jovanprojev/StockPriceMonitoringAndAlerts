using Microsoft.AspNetCore.SignalR;
using StockPriceMonitoringAndAlerts.Hubs;

namespace StockPriceMonitoringAndAlerts.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
