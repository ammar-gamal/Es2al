using Es2al.Models.Entites;

namespace Es2al.DataAccess.Repositories.IRepositroies
{
    public interface INotificationRepository:IBaseRepository<Notification>
    {
        IQueryable<Notification> GetUserNotifications(int userId);
        Task DeleteUserReadedNotificationsAsync(int userId);
        Task<Notification> GetNotification(int notificationId, int userId);
    }
}
