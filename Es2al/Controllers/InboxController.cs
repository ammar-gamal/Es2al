using Es2al.Filters;
using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Es2al.Services.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Es2al.Controllers
{
    [Authorize]
    public class InboxController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ITagService _tagService;
        public InboxController(IQuestionService questionService, ITagService tagService)
        {
            _questionService = questionService;
            _tagService = tagService;
        }
        [HttpGet("{username}/inbox", Name = "inbox")]
        public async Task<IActionResult> Index(QuestionFilterVM questionFilterVM, int pageIndex = 1)
        {
     
            var questions = await GetQuestionsAsync(questionFilterVM, pageIndex);
            ViewData["Filters"] = questionFilterVM;
            ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
            return View(questions);
        }
        [HttpGet("inbox/question-part", Name = "inbox/question-part")]
        public async Task<IActionResult> PartialQuestions(QuestionFilterVM questionFilterVM, int pageIndex)
        {
            var questions = await GetQuestionsAsync(questionFilterVM, pageIndex);
            return PartialView("~/Views/Question/_QuestionCards.cshtml", questions);
        }
        private async Task<PaginatedList<QuestionVM>> GetQuestionsAsync(QuestionFilterVM questionFilterVM, int pageIndex)
        {
            var userId = GetCurrentUserId();
            var questions = await _questionService.GetUserInboxAsync(userId, pageIndex, questionFilterVM);
            return questions;
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
