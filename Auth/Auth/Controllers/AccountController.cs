using System.Threading.Tasks;
using System.Web.Mvc;
using Data.Models;
using Auth.Services.Interfaces;
using Auth.ViewModels.Account;
using AutoMapper;
using Microsoft.AspNet.Identity;

namespace Auth.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        #region Constructors

        public AccountController()
        {
        }

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        #endregion


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region Actions

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _mapper.Map<RegisterViewModel, ApplicationUser>(model);

            var result = await _accountService.Register(user, model.Password);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var loginSucceeded = await _accountService.Login(model);

            if (loginSucceeded) return RedirectToLocal(returnUrl);
            ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło.");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            var user = await _accountService.FindUserByNameAsync(HttpContext.User.Identity.Name);
            var model = _mapper.Map<ApplicationUser, EditProfileViewModel>(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByNameAsync(HttpContext.User.Identity.Name);

            _mapper.Map(model, user);

            var result = await _accountService.UpdateUserAsync(user);

            if (result.Succeeded)
                ViewBag.Message = "Profil został zaktualizowany";
            else
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisu");

            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByNameAsync(HttpContext.User.Identity.Name);
            var result = await _accountService.ChangeUserPasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
                return RedirectToAction("Login", "Account");

            AddErrors(result);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmEmailApi()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");
            var result = await _accountService.ConfirmUserEmail(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByEmailAsync(model.Email);

            if (user == null || !await _accountService.IsUserEmailConfirmedAsync(user.Id))
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            await _accountService.SendPasswordResetEmailConfirmationLinkAsync(user.Id);
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByNameAsync(model.UserName);
            if (user == null)
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            var result = await _accountService.ResetUserPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Account");
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _accountService.Logout(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        ///     Do testowania roli usera
        ///     GET: /Account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}