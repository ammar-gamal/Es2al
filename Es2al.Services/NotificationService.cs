using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.ExtensionMethods;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Es2al.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserService _applicationUserService;
        private readonly INotificationHubService _notificationHubService;
        private readonly string _baseUrl;

        public NotificationService(IConfiguration cofiguration,
                                   IHttpContextAccessor httpContextAccessor,
                                   INotificationRepository notificationRepository,
                                   ApplicationUserService applicationUserService,
                                   INotificationHubService notificationHubService)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
            _applicationUserService = applicationUserService;
            _notificationHubService = notificationHubService;
            _baseUrl = cofiguration.GetSection("BaseUrl")?.Value
                  ?? throw new InvalidOperationException("BaseUrl configuration is missing");

        }

        public async Task<Dictionary<bool, List<Notification>>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUserNotifications(userId)
                                                .AsNoTracking()
                                                .GroupBy(g => g.IsMarkedAsReed)
                                                .ToDictionaryAsync(e => e.Key, e => e.OrderByDescending(i => i.Date).ToList());
        }
        public async Task DeleteUserReadedNotificationsAsync(int userId)
        {
            await _notificationRepository.DeleteUserReadedNotificationsAsync(userId);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetNotificationAsync(notificationId, userId);
            if (notification is null)
                throw new KeyNotFoundException($"Notification with ID {notificationId} not found for user {userId}");

            if (notification.IsMarkedAsReed)
                return;

            notification.IsMarkedAsReed = true;
            await _notificationRepository.UpdateAsync(notification);

        }
        private int GetCurrentUserId() => _httpContextAccessor.HttpContext.User.GetUserIdAsInt();
        private string GetCurrentUserName() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)!;

        public async Task AnswerNotificationAsync(int receiverId, int notificationRecipientId, int questionId)
        {
            var receiverName = await _applicationUserService.GetUserNameAsync(receiverId) ?? "Unknown User";
            var relatedUrl = $"{_baseUrl}/questions/find/{questionId}";
            var text = $"{receiverName} has answered your question.";
            var anchorText = "Click to see answer";

            await CreateAndSaveNotificationAsync(notificationRecipientId, text, anchorText, relatedUrl);
        }

        public async Task UnFollowNotificationAsync(int notificationRecipientId)
        {

            var currentUserId = GetCurrentUserId();
            var currentUserName = GetCurrentUserName();
            var relatedUrl = $"{_baseUrl}/profile/{currentUserId}";
            var text = $"{currentUserName} has unfollowed you.";
            var anchorText = $"Click to see {currentUserName}'s profile";

            await CreateAndSaveNotificationAsync(notificationRecipientId, text, anchorText, relatedUrl);
        }

        public async Task FollowNotificationAsync(int notificationRecipientId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserName = GetCurrentUserName();
            var relatedUrl = $"{_baseUrl}/profile/{currentUserId}";
            var text = $"{currentUserName} has followed you.";
            var anchorText = $"Click to see {currentUserName}'s profile";

            await CreateAndSaveNotificationAsync(notificationRecipientId, text, anchorText, relatedUrl);
        }

        private async Task CreateAndSaveNotificationAsync(int notificationRecipientId, string text, string anchorText, string relatedUrl)
        {
            var notification = new Notification
            {
                Date = DateTime.Now,
                Text = text,
                AnchorText = anchorText,
                UserId = notificationRecipientId,
                RelatedUrl = relatedUrl
            };

            await _notificationRepository.AddAsync(notification);
            await UpdateClientNotificationsNumberAsync(notificationRecipientId, notification);
        }
        //public async Task UnFollowNotificationAsync(int notificationRecipientId)
        //{
        //    string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
        //    int currentUserId = GetCurrentUserId();
        //    string currentUserName = GetCurrentUserName();
        //    string relatedUrl = $"{baseUrl}/profile/{currentUserId}";

        //    Notification notification = new Notification
        //    {
        //        Date = DateTime.UtcNow,
        //        Text = $"{currentUserName} Is Unfollowed You.",
        //        AnchorText = $"Click To See {currentUserName}'s Profile",
        //        UserId = notificationRecipientId,
        //        RelatedUrl = relatedUrl
        //    };
        //    await _notificationRepository.AddAsync(notification);
        //    await UpdateClientNotificationsNumberAsync(notificationRecipientId, notification);

        //}
        //public async Task FollowNotificationAsync(int notificationRecipientId)
        //{

        //    string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
        //    int currentUserId = GetCurrentUserId();
        //    string currentUserName = GetCurrentUserName();
        //    string relatedUrl = $"{baseUrl}/profile/{currentUserId}";

        //    Notification notification = new Notification
        //    {
        //        Date = DateTime.UtcNow,
        //        Text = $"{currentUserName} Is Followed You.",
        //        AnchorText = $"Click To See {currentUserName}'s Profile",
        //        UserId = notificationRecipientId,
        //        RelatedUrl = relatedUrl
        //    };
        //    await _notificationRepository.AddAsync(notification);
        //    await UpdateClientNotificationsNumberAsync(notificationRecipientId, notification);
        //}
        //public async Task AnswerNotificationAsync(int receiverId, int notificationRecipientId, int questionId)
        //{
        //    string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
        //    string relatedUrl = $"{baseUrl}/questions/find/{questionId}";
        //    string ReceiverName = (await _applicationUserService.GetUserNameAsync(receiverId))!;
        //    Notification notification = new Notification
        //    {
        //        Date = DateTime.Now,
        //        Text = $"{ReceiverName} Is Answered Your Question.",
        //        AnchorText = "Click To See Answer",
        //        UserId = notificationRecipientId,
        //        RelatedUrl = relatedUrl
        //    };

        //    await _notificationRepository.AddAsync(notification);
        //    await UpdateClientNotificationsNumberAsync(notificationRecipientId, notification);


        //}

        public async Task<int> NumberOfUnReadedNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUserNotifications(userId)
                                                .Where(e => e.IsMarkedAsReed == false)
                                                .CountAsync();
        }
        private async Task UpdateClientNotificationsNumberAsync(int userId, Notification? notification)
        {
            var currentNotificationsCount = await NumberOfUnReadedNotificationsAsync(userId);
            await _notificationHubService.UpdateClientNotificationsNumberAsync(userId.ToString(),
                                        new { count = currentNotificationsCount, notification });
        }
    }
}

