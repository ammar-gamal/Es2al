using Es2al.Models.Entites;

namespace Es2al.Services.IServices
{
    public interface INotificationService
    {
        public Task<Dictionary<bool, List<Notification>>> GetUserNotificationsAsync(int userId);
        public Task DeleteUserReadedNotifications(int userId);
        public Task<int> NumberOfUnReadedNotificationsAsync(int userId);
        public Task MarkNotificationAsRead(int notificationId,int userId);
    }
}
