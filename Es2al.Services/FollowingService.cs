using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.CustomException;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Es2al.Services.Events.CustomEventArgs;
using static Es2al.Services.Events.AsyncEventHandlers;

namespace Es2al.Services
{
    public class FollowingService : IFollowingService
    {
        private readonly IUserFollowRepository _userFollowRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public event EventHandlerAsync<FollowingEventArgs> OnUserFollow;
        public event EventHandlerAsync<FollowingEventArgs> OnUserUnFollow;

        public FollowingService(IUserFollowRepository userFollowRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userFollowRepository = userFollowRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task FollowAsync(int followerId, int followingId)
        {
            if (followingId == followerId)
                throw new AppException("Can't Follow Your Self");
            bool IsFollowing = await IsFollowingAsync(followerId, followingId);
            if (IsFollowing)
                throw new AppException("Already Following This User");
            using (var transaction = await _userFollowRepository.BeginTransactionAsync())
            {
                try
                {
                    await _userFollowRepository.AddAsync(new UserFollow() { FollowerId = followerId, FollowingId = followingId });
                    if (OnUserFollow != null)
                        await OnUserFollow.Invoke(this, new FollowingEventArgs { UserId=followingId});

                    await transaction.CommitAsync();

                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new AppException(exception.Message);
                }

            }
        }
        public async Task UnFollowAsync(int followerId, int followingId)
        {
            if (followingId == followerId)
                throw new AppException("Can't UnFollow Your Self");

            bool IsFollowing = await IsFollowingAsync(followerId, followingId);

            if (!IsFollowing)
                throw new AppException("You Are Not Following This User");
            using (var transaction = await _userFollowRepository.BeginTransactionAsync())
            {
                try
                {
                    await _userFollowRepository.RemoveAsync(new UserFollow() { FollowerId = followerId, FollowingId = followingId });
                    if (OnUserUnFollow != null)
                    {
                        string followerName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)!;
                        await OnUserUnFollow.Invoke(this, new FollowingEventArgs { UserId=followingId});
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    throw new AppException(exception.Message);
                }
            }

        }
        public async Task<PaginatedList<DisplayUserVM>> GetUserFollowersAsync(int userId, int pageIndex)
        {
            var list = _userFollowRepository.GetUserFollowers(userId)
                                            .Select(e => new DisplayUserVM { Id = e.FollowerId, Image = e.Follower.Image, UserName = e.Follower.UserName })
                                            .OrderBy(e => e.Id);
            return await PaginatedList<DisplayUserVM>.CreateAsync(list, pageIndex, Constants.ItemsPerPage);
        }

        public async Task<PaginatedList<DisplayUserVM>> GetUserFollowingsAsync(int userId, int pageIndex)
        {
            var list = _userFollowRepository.GetUserFolloweings(userId)
                                            .Select(e => new DisplayUserVM { Id = e.FollowingId, Image = e.Following.Image, UserName = e.Following.UserName })
                                            .OrderBy(e=>e.Id);
            return await PaginatedList<DisplayUserVM>.CreateAsync(list, pageIndex, Constants.ItemsPerPage);
        }


        public async Task<bool> IsFollowingAsync(int followerId, int followingId) =>
          await _userFollowRepository.GetAll().ContainsAsync(new UserFollow() { FollowerId = followerId, FollowingId = followingId });
    }
}
