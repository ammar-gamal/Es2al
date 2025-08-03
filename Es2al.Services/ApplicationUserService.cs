using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.Paging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Es2al.Services
{

    public class ApplicationUserService : UserManager<AppUser>
    {

        public ApplicationUserService(IUserStore<AppUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<AppUser> passwordHasher,
        IEnumerable<IUserValidator<AppUser>> userValidators,
        IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<AppUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        { }
        public async Task<UserProfileVM?> GetUserProfileVMByIdAsync(int userId)
        {
            //string normalized = username.ToUpper();
            UserProfileVM? profile = await Users

                  .Where(e => e.Id == userId)
                  .Select(e => new UserProfileVM()
                  {
                      Id = e.Id,
                      UserName = e.UserName!,
                      Email = e.Email!,
                      Image = e.Image,
                      Bio = e.Bio,
                      FollowerAndFollowingVM = new FollowerAndFollowingVM()
                      {
                          FollowersCount = e.Followers.Count,
                          FollowingsCounts = e.Followings.Count
                      },
                      Tags = e.Tags.Select(e => e.Tag.Name).ToList(),
                      AnsweredQuestionsCount=e.ReceivedQuestions.Where(q=>q.IsAnswered==true).Count()
                  }).FirstOrDefaultAsync();
            return profile;

        }
        public async Task<EditUserVM?> GetEditUserVMAsync(int userId)
        {
            EditUserVM? profile = await Users.Include(e => e.Tags)
                  .ThenInclude(e => e.Tag)
                  .Where(e => e.Id == userId)
                  .Select(e => new EditUserVM()
                  {
                      UserName = e.UserName!,
                      Email = e.Email!,
                      Image = e.Image,
                      Bio = e.Bio,
                      Tags = e.Tags.Select(e => e.Tag.Id).ToHashSet()
                  }).FirstOrDefaultAsync();
            return profile;

        }
        public async Task<IdentityResult?> UpdateUserAsync(int userId, EditUserVM userVM)
        {
            AppUser? appUser = await Users.Include(e => e.Tags)
                                          .FirstOrDefaultAsync(e => e.Id == userId);
            if (appUser is null)
                return null;

            appUser.Email = userVM.Email;
            appUser.Bio = userVM.Bio;
            appUser.Tags.Clear();

            if (userVM.Image is not null)
                appUser.Image = userVM.Image;

            if (userVM.Tags is not null)
            {
                foreach (var tagId in userVM.Tags)
                {
                    appUser.Tags.Add(new UserTag() { TagId = tagId });
                }
            }
            IdentityResult? result = await UpdateAsync(appUser);
            return result;
        }
        public async Task<PaginatedList<DisplayUserVM>> SearchUsersByUsername(string username, int pageIndex)
        {
            var matchedUsers = Users.Where(e => e.UserName!.StartsWith(username))
                                  .OrderByDescending(e => e.Followers.Count)
                                  .Select(e => new DisplayUserVM
                                  {
                                      Id = e.Id,
                                      UserName = e.UserName!,
                                      Image = e.Image!
                                  });
            return await PaginatedList<DisplayUserVM>.CreateAsync(matchedUsers, pageIndex, Constants.ItemsPerPage);
        }
        
        public async Task<string?>GetUserNameAsync(int Id)
        {
            return await Users.Where(e => e.Id == Id).Select(e => e.UserName).FirstOrDefaultAsync();
        }


    }
}
