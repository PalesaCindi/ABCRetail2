using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuditLogService _auditLogService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AuditLogService auditLogService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditLogService = auditLogService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // model is passed back, so FullName/Email are repopulated in the view
                return View(model);
            }

            var existingUser =
                await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    model.Password);

            if (result.Succeeded)
            {
                // Record new user registration in Azure Files
                await _auditLogService.LogAsync(
                    $"New user registered: {model.Email}");

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                // Record successful login
                await _auditLogService.LogAsync(
                    $"User logged in: {model.Email}");

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _auditLogService.LogAsync(
                    $"User logged in: {model.Email}");

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (result.IsLockedOut)
            {
                await _auditLogService.LogAsync(
                    $"Account locked out after repeated failed login attempts: {model.Email}");

                ModelState.AddModelError(
                    "",
                    "This account has been locked out due to multiple failed login attempts. Please try again later.");

                return View(model);
            }

            ModelState.AddModelError(
                "",
                "Invalid email or password.");

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            string userEmail =
                User.Identity?.Name ?? "Unknown user";

            await _signInManager.SignOutAsync();

            await _auditLogService.LogAsync(
                $"User logged out: {userEmail}");

            return RedirectToAction(
                "Index",
                "Home");
        }
    }
}