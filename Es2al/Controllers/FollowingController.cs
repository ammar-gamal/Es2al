using Es2al.Services.ExtensionMethods;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Es2al.Controllers
{
    [Authorize]
    public class FollowingController : Controller
    {
        private readonly IFollowingService _followingService;
        public FollowingController(IFollowingService followingService)
        {
            _followingService = followingService;
        }
        [HttpPost("follow/{id}")]
        public async Task<IActionResult> Follow(int id)
        {
            //int currentUserId = GetCurrentUserId();
            int currentUserId = User.GetUserIdAsInt();
      

            await _followingService.FollowAsync(currentUserId, id);
            return Ok();
        }
        [HttpPost("unfollow/{id}")]
        public async Task<IActionResult> UnFollow(int id)
        {
            int currentUserId = User.GetUserIdAsInt();
            await _followingService.UnFollowAsync(currentUserId, id);

            return Ok();
        }
        [HttpGet("followers/{id}")]
        public async Task<IActionResult> Followers(int id, int pageIndex)
        {
            var followersList = await _followingService.GetUserFollowersAsync(id, pageIndex);
            return PartialView("~/Views/Search/_UsersList.cshtml", followersList);
        }
        [HttpGet("followings/{id}")]
        public async Task<IActionResult> Followings(int id, int pageIndex)
        {
            var followingsList = await _followingService.GetUserFollowingsAsync(id, pageIndex);
            return PartialView("~/Views/Search/_UsersList.cshtml", followingsList);
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
