using Es2al.Services.IServices;
using Microsoft.AspNetCore.SignalR;

namespace Es2al.Hubs.Services
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        public NotificationHubService(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }
        public async Task UpdateClientNotificationsNumberAsync(string userId,object content)
        {
            await _notificationHub.Clients.User(userId).SendAsync("notifyUser", content);
        }
    }
}
