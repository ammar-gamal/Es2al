using Es2al.Services.ExtensionMethods;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Es2al.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task MarkNotificationAsRead(int notificationId)
        {
            string userId = Context.UserIdentifier!;
            await _notificationService.MarkNotificationAsReadAsync(notificationId, Int32.Parse(userId));
            var notificationsCounts = await GetNumberOfUnrededNotificationsAsync(Int32.Parse(userId));
            await Clients.User(userId).SendAsync("changeNotificationPlace", new { notificationId, count = notificationsCounts });
            await Clients.User(userId).SendAsync("notifyUser", new { count = notificationsCounts });
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            string userId = Context.UserIdentifier!;
            var notificationsCounts = await GetNumberOfUnrededNotificationsAsync(Int32.Parse(userId));
            await Clients.Caller.SendAsync("setNotificationCount", new { count = notificationsCounts });
        }
        private async Task<int> GetNumberOfUnrededNotificationsAsync(int userId) => await _notificationService.NumberOfUnReadedNotificationsAsync(userId);
    }
}
