using Es2al.Models.Entites;
using Es2al.Services.ViewModels;
using Es2al.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Es2al.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationUserService _applicationUserService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(ApplicationUserService applicationUserService, SignInManager<AppUser> signInManager)
        {
            _applicationUserService = applicationUserService;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("register", Name = "register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet("login",Name ="login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("register",Name ="register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    Email = registerVM.Email,
                    UserName = registerVM.UserName,
                };
                IdentityResult result = await _applicationUserService.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    await _applicationUserService.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Feed");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(registerVM);
        }
        [HttpPost("login",Name ="login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                AppUser? user;
                user = await _applicationUserService.FindByEmailAsync(loginVM.EmailOrUserName) ??
                       await _applicationUserService.FindByNameAsync(loginVM.EmailOrUserName);

                if (user is null)
                {
                    ModelState.AddModelError("EmailOrUserName", "Invalid email or username.");
                    return View(loginVM);
                }
                var isPasswordValid = await _applicationUserService.CheckPasswordAsync(user, loginVM.Password);

                if (isPasswordValid)
                {
                    await _signInManager.SignInAsync(user, loginVM.RememberMe);
                    return RedirectToAction("Index","Feed");
                }
                else
                    ModelState.AddModelError("Password", "Incorrect password.");
            }
            return View(loginVM);
        }
        [HttpGet("logout",Name ="logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

       
    }
}