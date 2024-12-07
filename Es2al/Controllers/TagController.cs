using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace Es2al.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("all-tags", Name = "all-Tags")]
        public async Task<IActionResult> GetTags()
        {
            ModelState.Clear();
            return PartialView("_Tags", await _tagService.GetAllTagsAsync());
        }
        [HttpPost("create-tag", Name = "create-tag")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { errors = new { name = "Tag name cannot be empty." } });//To Avoid Breaking The PRG
            }

            bool isExist = await _tagService.IsTagNameExistAsync(name);
            if (isExist)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new {errors = new { name = "The Tag Name Is Exist Before." } });//To Avoid Breaking The PRG
            }

            await _tagService.AddTagAsync(new Tag { Name = name });
            return RedirectToAction("GetTags");
        }
        [HttpGet("delete-tag/{id}",Name ="delete-tag")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.RemoveTagAsync(id);
            return RedirectToAction("index");
        }
        [HttpGet("update-tag", Name = "update-tag")]

        public async Task<IActionResult> Update(int id)
        {
            var tag=await _tagService.GetTagAsync(id);
            return View("update", tag);

        }
        [HttpPost("update-tag",Name ="update-tag")]
        public async Task<IActionResult> Update(Tag tag)
        {
          
            if (String.IsNullOrWhiteSpace(tag.Name))
            {
                ModelState.AddModelError("name", "The Tag Cannot Be Empty");
                return View("update", tag);

            }
            var flag = await _tagService.CannotUpdateTagAsync(tag);
            if(flag){ 

                ModelState.AddModelError("name", "The Tag Name Is Exist Before");
                return View("update", tag);

            }
            await _tagService.UpdateTagAsync(tag);
            return RedirectToAction("index");

        }

    }
}
