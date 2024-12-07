using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Es2al.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ITagService _tagService;
        public FeedController(IQuestionService questionService, ITagService tagService)
        {
            _questionService = questionService;
            _tagService = tagService;

        }
        [HttpGet("",Name ="feed")]
        public async Task<IActionResult> Index(QuestionFilterVM questionFilterVM,int pageIndex = 1)
        {
            var QAs = await GetQAsAsync(pageIndex, questionFilterVM);
            ViewData["Filters"] = questionFilterVM;
            ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
            return View("index", QAs);
        }
        [HttpGet("feed/question-part", Name = "feed/question-part")]
        public async Task<IActionResult> PartialFeed(int pageIndex,QuestionFilterVM questionFilterVM)
        {
            var QAs = await GetQAsAsync(pageIndex, questionFilterVM);
            return PartialView("~/Views/Question/_QuestionAnswerCards.cshtml", QAs);
        }
        private async Task<PaginatedList<QuestionAnswerVM>> GetQAsAsync(int pageIndex, QuestionFilterVM questionFilterVM)
        {
            var userId = GetCurrentUserId();
            var QAs = await _questionService.GetFeedQAsAsync(userId, pageIndex,questionFilterVM);
            return QAs;
        }
        private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
