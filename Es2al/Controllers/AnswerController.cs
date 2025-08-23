using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Es2al.Models.Entites;
using Microsoft.AspNetCore.Authorization;
using Es2al.Services.ExtensionMethods;
namespace Es2al.Controllers
{
    [Authorize]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }
        public IActionResult AnswerForm()
        {
            return PartialView("_AnswerForm");
        }

        [HttpPost("inbox-answer", Name = "inbox-answer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InboxAnswer(NewAnswerVM answerVM)
        {

            if (ModelState.IsValid)
            {
                int userId = User.GetUserIdAsInt();
                Answer answer = GetAnswer(answerVM, userId, answerVM.QuestionId);
                await _answerService.SaveAnswerAsync(answer);
                return Ok();
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.PartialContent;
            ViewData["questionId"] = answerVM.QuestionId;
            return PartialView("_AnswerForm", answerVM);

        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private Answer GetAnswer(NewAnswerVM answerVM, int userId, int questionId) => new Answer
        {
            Date = DateTime.Now,
            Text = answerVM.Text,
            UserId = userId,
            QuestionId = questionId

        };

    }
}
