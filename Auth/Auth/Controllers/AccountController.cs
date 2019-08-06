using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using Auth.ViewModels.Account;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;

namespace Auth.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController()
        {
            
        }

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }


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

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var user = await _userManager.FindAsync(model.UserName, model.Password);
        //    if (user != null)
        //    {
        //        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //        await SignInAsync(user, model.RememberMe);
        //        return RedirectToLocal(returnUrl);
        //    }

        //    ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło.");
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> EditProfile()
        //{
        //    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        //    var model = new EditProfileViewModel
        //    {
        //        BirthDate = user.BirthDate,
        //        LastName = user.LastName,
        //        FirstName = user.FirstName
        //    };
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditProfile(EditProfileViewModel model)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        //    user.FirstName = model.FirstName;
        //    user.LastName = model.LastName;
        //    user.BirthDate = model.BirthDate;

        //    var result = await _userManager.UpdateAsync(user);

        //    if (result.Succeeded)
        //        ViewBag.Message = "Profil został zaktualizowany";
        //    else
        //        ModelState.AddModelError("", "Wystąpił błąd podczas zapisu");

        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        //        var result = await _userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
        //        if (result.Succeeded)
        //        {
        //            AuthenticationManager.SignOut();
        //            return RedirectToAction("Login", "Account");
        //        }

        //        AddErrors(result);
        //    }

        //    return View(model);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ConfirmEmailApi()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //        return View("Error");
        //    var result = await _userManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null || !await _userManager.IsEmailConfirmedAsync(user.Id))
        //        // Don't reveal that the user does not exist or is not confirmed
        //        return View("ForgotPasswordConfirmation");

        //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //    // Send an email with this link
        //    var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
        //    var callbackUrl = Url.Action("ResetPassword", "Account", new {userId = user.Id, code}, Request.Url.Scheme);
        //    await _userManager.SendEmailAsync(user.Id, "Reset hasła",
        //        "Zresetuj hasło, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");
        //    return RedirectToAction("ForgotPasswordConfirmation", "Account");

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var user = await _userManager.FindByNameAsync(model.UserName);
        //    if (user == null)
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    AddErrors(result);
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Logout()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Login", "Account");
        //}

        ///// <summary>
        ///// Do testowania roli usera
        ///// GET: /Account
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Authorize(Roles = "User")]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        #endregion


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        //    return RedirectToAction("Index", "Home");
        //}

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        //}

        //private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        //{
        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(userID);
        //    var callbackUrl = Url.Action("ConfirmEmail", "Account",
        //        new {userId = userID, code}, Request.Url.Scheme);
        //    await _userManager.SendEmailAsync(userID, subject,
        //        "Potwierdź swoje konto, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");

        //    return callbackUrl;
        //}

        #endregion
    }
}