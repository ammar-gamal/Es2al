using Es2al.Services.ViewModels;
using Es2al.Services.Paging;
using static Es2al.Services.Utilites;
using Es2al.Services.CustomEventArgs;

namespace Es2al.Services.IServices
{
    public interface IFollowingService
    {
        Task FollowAsync(int followerId, int followingId);
        Task UnFollowAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<PaginatedList<DisplayUserVM>> GetUserFollowingsAsync(int userId, int pageIndex);
        Task<PaginatedList<DisplayUserVM>> GetUserFollowersAsync(int userId, int pageIndex);
        public event EventHandlerAsync<FollowingEventArgs> OnUserFollow;
        public event EventHandlerAsync<FollowingEventArgs> OnUserUnFollow;

    }
}
