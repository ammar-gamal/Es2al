using Es2al.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Es2al.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ApplicationUserService _applicationUserService;
        public SearchController(ApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("users-search",Name = "users-search")]
        public async Task<IActionResult> SearchByUsername(string username,int pageIndex)
        {

            var res= await _applicationUserService.SearchUsersByUsername(username, pageIndex);
            return PartialView("_UsersList", res);
        }
    }
}
