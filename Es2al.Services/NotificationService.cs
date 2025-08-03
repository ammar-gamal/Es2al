using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.Events.CustomEventArgs;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserService _applicationUserService;
        public NotificationService(IConfiguration cofiguration,
                                   IHttpContextAccessor httpContextAccessor,
                                   INotificationRepository notificationRepository,
                                   IAnswerService answerService,
                                   IFollowingService followingService,
                                   ApplicationUserService applicationUserService)
        {
            _notificationRepository = notificationRepository;
            _configuration = cofiguration;
            _httpContextAccessor = httpContextAccessor;
            _applicationUserService = applicationUserService;
            answerService.OnQuestionAnswer += AnswerNotificationAsync;
            followingService.OnUserUnFollow += UnFollowNotificationAsync;
            followingService.OnUserFollow += FollowNotificationAsync;
            
        }

        public async Task<Dictionary<bool, List<Notification>>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUserNotifications(userId)
                                                .AsNoTracking()
                                                .GroupBy(e => e.IsMarkedAsReed)
                                                .ToDictionaryAsync(e => e.Key, e => e.ToList());
        }
        public async Task DeleteUserReadedNotifications(int userId)
        {
            await _notificationRepository.DeleteUserReadedNotificationsAsync(userId);
        }

        public async Task MarkNotificationAsRead(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetNotification(notificationId, userId);
            notification.IsMarkedAsReed = true;
            await _notificationRepository.UpdateAsync(notification);

        }
        private int GetCurrentUserId() => Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetCurrentUserName() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)!;
        private async Task AnswerNotificationAsync(object? sender, QuestionAnswerEventArgs eventArgs)
        {
            string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
            string relatedUrl = $"{baseUrl}/questions/find/{eventArgs.QuestionId}";
            string ReceiverName = (await _applicationUserService.GetUserNameAsync(eventArgs.ReceiverId))!;
            Notification notification = new Notification
            {
                Date = DateTime.Now,
                Text = $"{ReceiverName} Is Answered Your Question.",
                AnchorText = "Click To See Answer",
                UserId = eventArgs.UserId,
                RelatedUrl = relatedUrl
            };
            await _notificationRepository.AddAsync(notification);
        }
        private async Task UnFollowNotificationAsync(object? sender, FollowingEventArgs eventArgs)
        {
            string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
            int currentUserId = GetCurrentUserId();
            string currentUserName = GetCurrentUserName();
            string relatedUrl = $"{baseUrl}/profile/{currentUserId}";

            Notification notification = new Notification
            {
                Date = DateTime.UtcNow,
                Text = $"{currentUserName} Is Unfollowed You.",
                AnchorText = $"Click To See {currentUserName}'s Profile",
                UserId = eventArgs.UserId,
                RelatedUrl = relatedUrl
            };
            await _notificationRepository.AddAsync(notification);
        }
        private async Task FollowNotificationAsync(object? sender, FollowingEventArgs eventArgs)
        {
            string baseUrl = _configuration.GetSection("BaseUrl")?.Value ?? "Not Defined";
            int currentUserId = GetCurrentUserId();
            string currentUserName = GetCurrentUserName();
            string relatedUrl = $"{baseUrl}/profile/{currentUserId}";

            Notification notification = new Notification
            {
                Date = DateTime.UtcNow,
                Text = $"{currentUserName} Is Followed You.",
                AnchorText = $"Click To See {currentUserName}'s Profile",
                UserId = eventArgs.UserId,
                RelatedUrl = relatedUrl
            };
            await _notificationRepository.AddAsync(notification);
        }

        public async Task<int> NumberOfUnReadedNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUserNotifications(userId)
                                                .Where(e=>e.IsMarkedAsReed==false)
                                                .CountAsync();
        }
    }
}
