using Es2al.Models.Enums;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Security.Claims;

namespace Es2al.Controllers
{
    [Authorize]
    public class ReactionController : Controller
    {
        private readonly IReactionService _reactionService;
        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }
        //Return 1 => indicate for no previous reaction Save the reaction for answer
        //Return -1 => indicate for the same reaction was exist before and pressed again  remove the reaction 
        //Return 0 => indacate for the user was make a reaction and pressed the another reaction   remove the reaction from the previous one and add it to another 
        [HttpPost("react", Name = "react")]
        public async Task<IActionResult> React(int answerId, string react)
        {
            var res = await _reactionService.ReactAsync(GetCurrentUserId(), answerId,react);
            return Ok(res);
        }
        private int GetCurrentUserId() => Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
