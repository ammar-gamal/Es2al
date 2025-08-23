using Es2al.Models.Entites;
using Es2al.Services.Events.CustomEventArgs;
using Microsoft.Extensions.Configuration;

namespace Es2al.Services.IServices
{
    public interface INotificationService
    {
        public Task<Dictionary<bool, List<Notification>>> GetUserNotificationsAsync(int userId);
        public Task DeleteUserReadedNotificationsAsync(int userId);
        public Task<int> NumberOfUnReadedNotificationsAsync(int userId);
        public Task MarkNotificationAsReadAsync(int notificationId,int userId);
        public Task AnswerNotificationAsync(int receiverId, int notificationRecipientId, int questionId);
        public Task UnFollowNotificationAsync(int notificationRecipientId);
        public Task FollowNotificationAsync(int notificationRecipientId);
    }
}
