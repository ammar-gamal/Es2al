using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Es2al.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("notifications",Name = "notifications")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(GetCurrentUserId());
            return View(notifications);
        }
        [HttpGet("notifications/delete-all-readed", Name = "delete-all-readed")]
        public async Task<IActionResult> DeleteReadedNotifications()
        {
            await _notificationService.DeleteUserReadedNotifications(GetCurrentUserId());
            return RedirectToAction("index");
        }
        [HttpPost("notifications/mark-read/{id}", Name = "mark-notification")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            await _notificationService.MarkNotificationAsRead(id, GetCurrentUserId());
            var notifications = await _notificationService.GetUserNotificationsAsync(GetCurrentUserId());
            return Ok();
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
