using Es2al.Filters;
using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Es2al.Services;

namespace Es2al.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ITagService _tagService;
        private readonly IQuestionTagService _questionTagService;
        public QuestionController(IQuestionService questionService,IQuestionTagService questionTagService, ITagService tagService)
        {
            _questionService = questionService;
            _tagService = tagService;
            _questionTagService = questionTagService;
        }
        [HttpGet("question", Name = "question")]
        public async Task<IActionResult> Question()
        {
            ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
            return PartialView("_QuestionForm", new NewQuestionVM());
        }
        [HttpPost("ask", Name = "ask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(NewQuestionVM questionVM)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear(); //When posting data and then re-displaying data in the same request, the ModelState will be populated with the data from the original post.
                int senderId = GetCurrentUserId();
                Question question = new();
                var selectedTags = questionVM.Tags.Select(tagId => new QuestionTag() { TagId = tagId }).ToList();
                question.Text = questionVM.Text;
                question.IsAnonymous = questionVM.IsAnonymous;
                question.ReceiverId = questionVM.ReceiverId;
                question.SenderId = senderId;
                question.Date = DateTime.Now;
                question.Tags = selectedTags;
                await _questionService.CreateQuestionAsync(question);
                return RedirectToAction("Question");
            }
            ViewData["AllTags"] = await _tagService.GetAllTagsAsync();
            return PartialView("~/Views/Question/_QuestionForm.cshtml", questionVM);
        }
        [HttpPost("sub-question", Name = "sub-question")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskSubQuestion(SubQuestionVM questionVM)
        {
            ViewData["questionId"] = questionVM.ParentQuestionId;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                int senderId = GetCurrentUserId();
                Question question = new()
                {
                    SenderId = senderId,
                    ReceiverId = questionVM.ReceiverId,
                    Text = questionVM.Text,
                    IsAnonymous = questionVM.IsAnonymous,
                    Date = DateTime.Now,
                    ParentQuestionId = questionVM.ParentQuestionId,
                    ThreadId = questionVM.ThreadId,
                    Tags = (await _questionTagService.GetQuestionTagsAsync(questionVM.ParentQuestionId))!
                };

                await _questionService.CreateQuestionAsync(question);
            
                return PartialView("_SubQuestion", new SubQuestionVM());
            }

            // Return partial view and 206 status code for validation failure
            return PartialView("_SubQuestion", questionVM);
        }

        [HttpGet("questions/find/{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var QA =await _questionService.GetQuestionAnswerAsync(id, GetCurrentUserId());
            return View("QuestionAnswer", QA);
        }
        [HttpGet("questions/delete/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            await _questionService.DeleteQuestionAsync(id,GetCurrentUserId());
            return Ok();
        }
        [HttpGet("questions/thread/{threadId:int:min(1)}/{questionId:int:min(1)}")]
        public async Task<IActionResult> QuestionsThread(int threadId,int questionId)
        {
            var res = await _questionService.GetQuestionsInThreadAsync(threadId, GetCurrentUserId());

            if (res == null)
                return RedirectToAction("index", "feed");

            ViewData["FocusQuestionId"] = questionId;
            return View("QuestionsThread", res);
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
