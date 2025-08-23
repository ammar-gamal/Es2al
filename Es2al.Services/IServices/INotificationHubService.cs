
namespace Es2al.Services.IServices
{
    public interface INotificationHubService
    {
        Task UpdateClientNotificationsNumberAsync(string userId, object content);
    }
}
