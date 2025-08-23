using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;


namespace Es2al.DataAccess.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        { }
        public IQueryable<Notification> GetUserNotifications(int userId)
        {
            return _dbSet.Where(e => e.UserId == userId);
        }

        public async Task DeleteUserReadedNotificationsAsync(int userId)
        {
            await GetUserNotifications(userId)
                 .Where(e => e.IsMarkedAsReed)
                 .ExecuteDeleteAsync();
        }

        public async Task<Notification?> GetNotificationAsync(int notificationId, int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == notificationId && e.UserId == userId);
        }
    }
}
