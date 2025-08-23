using Es2al.Services;
using Es2al.Services.ExtensionMethods;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Es2al.Services.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Es2al.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationUserService _applicationUserService;
        private readonly IFollowingService _followingService;
        private readonly ITagService _tagService;
        private readonly IQuestionService _questionService;

        public ProfileController(ApplicationUserService applicationUserSerivce, ITagService tagService, IFollowingService followingService, IQuestionService questionService)
        {
            _applicationUserService = applicationUserSerivce;
            _tagService = tagService;
            _followingService = followingService;
            _questionService = questionService;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("profile/{id}/questions",Name ="profile/questions")]
        public async Task<IActionResult> UserQuestions(int id, QuestionFilterVM questionFilterVM,int pageIndex=1)
        {
            var QAs = await GetQAsAsync(pageIndex,id, questionFilterVM);
            ViewData["Filters"] = questionFilterVM;
            ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
            ViewData["UserId"] = id;
            return View("questions", QAs);

        }
           
        [HttpGet("profile/{id}/question-part", Name = "profile/question-part")]
        public async Task<IActionResult> PartialFeed(int id,int pageIndex, QuestionFilterVM questionFilterVM)
        {
            var QAs = await GetQAsAsync(pageIndex, id, questionFilterVM);
            return PartialView("~/Views/Question/_QuestionAnswerCards.cshtml", QAs);
        }
      
        [HttpGet("profile/{id}", Name = "profile")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Show(int id)
        {

            UserProfileVM? user = await _applicationUserService.GetUserProfileVMByIdAsync(id);
            if (user is not null)
            {
                ModelState.Clear();
                ViewData["IsFollowing"] = await _followingService.IsFollowingAsync(User.GetUserIdAsInt(), id);
                ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
                return View("show", user);
            }
            else
                return Content($"{id} Is Not Found");

        }
        [HttpGet("profile/edit", Name = "profile/edit")]
        public async Task<IActionResult> Edit()
        {
            EditUserVM? user = await _applicationUserService.GetEditUserVMAsync(User.GetUserIdAsInt());
            ViewBag.AllTags = await _tagService.GetAllTagsAsync();
            return View("edit", user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(IFormFile? image, EditUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    using (MemoryStream stream = new())
                    {
                        await image.CopyToAsync(stream);
                        userVM.Image = stream.ToArray();
                    }
                }
                int currentUserId = User.GetUserIdAsInt(); 
                IdentityResult? result = await _applicationUserService.UpdateUserAsync(currentUserId, userVM);
                if (result is null)
                    return RedirectToAction("logout", "account");
                else if (result.Succeeded) {
                    return RedirectToAction("show", new { id = currentUserId });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }

            }
            ViewBag.AllTags = await _tagService.GetAllTagsAsync();
            return View("edit", userVM);
        }
        private async Task<PaginatedList<QuestionAnswerVM>> GetQAsAsync(int pageIndex, int userId, QuestionFilterVM questionFilterVM)
        {
            int visiterId = User.GetUserIdAsInt();
            var QAs = await _questionService.GetUserQA(visiterId,userId, pageIndex, questionFilterVM);
            return QAs;
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
