using BookStore.Common;
using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Messaging;
using BookStore.Core.Models.User;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static BookStore.Common.GlobalConstants;
using static BookStore.Common.GlobalExceptions;

namespace BookStore.Controllers
{
    public class UserController : BaseController
    {
        private const string Subject = "Verify Email";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;

        public UserController(
            SignInManager<ApplicationUser> _signInManager,
            UserManager<ApplicationUser> _userManager,
            IUserService _userService,
            IEmailSender _emailSender)
        {
            this.signInManager = _signInManager;
            this.userManager = _userManager;
            userService = _userService;
            emailSender = _emailSender;
        }

        public static string UserControllerName => nameof(UserController).Replace("Controller", string.Empty);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
            }

            var model = new LoginViewModel();

            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
                }
            }

            ModelState.AddModelError(string.Empty, GlobalExceptions.InvalidLogin);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
            }

            var model = new RegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await userService.CheckIfEmailIsFree(model.Email))
            {
                TempData[MessageConstant.ErrorMessage] = EmailAlreadyTaken;

                return View(model);
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var model = await userService.GetUserProfileDataAsync(GetCurrentUserId());

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), UserControllerName, new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                TempData[MessageConstant.ErrorMessage] = $"Error from external provider: {remoteError}";

                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData[MessageConstant.ErrorMessage] = "Error loading external login information.";
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                TempData[MessageConstant.WarningMessage] = NoLoginFoundPleaseRegister;

                var model = new RegisterViewModel()
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                };

                return View(nameof(Register), model);
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmail()
        {
            var model = new EmailConfirmViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmViewModel model, string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userEmail == null)
            {
                TempData[MessageConstant.ErrorMessage] = UserNotFound;

                return RedirectToAction(nameof(Profile));
            }


            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                TempData[MessageConstant.ErrorMessage] = UserNotFound;

                return RedirectToAction(nameof(Profile));
            }


            await this.userManager.ConfirmEmailAsync(user, model.UserCode);

            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> SendToken(string userEmail)
        {
            if (userEmail == null)
            {
                TempData[MessageConstant.ErrorMessage] = UserNotFound;

                return RedirectToAction(nameof(Profile));
            }

            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                TempData[MessageConstant.ErrorMessage] = UserNotFound;

                return RedirectToAction(nameof(Profile));
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            await this.emailSender
                .SendEmailAsync(BookStoreEmailAddress, AppName, userEmail, Subject, code);

            return RedirectToAction(nameof(ConfirmEmail));
        }
    }
}
