using System;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.Models;
using Auth.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace Auth.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; }

        #region REST API

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName,
                BirthDate = model.BirthDate
            };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return GetErrorResult(result);

            await UserManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Role, "User"));

            await SendEmailConfirmationTokenAsync(user.Id, "Potwierdź swoje konto");

            return Ok();
        }

        // GET api/Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [Route(Name = "ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "userID i code są wymagane");
                return BadRequest(ModelState);
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (!result.Succeeded)
            {
                GetErrorResult(result);
                return BadRequest();
            }

            var homeUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return Redirect($"{homeUrl}/Account/ConfirmEmailApi");
        }

        // POST api/Account/EditProfile
        [HttpPost]
        [Route("EditProfile")]
        public async Task<IHttpActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return null;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) return GetErrorResult(result);

            return Ok();
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded) return GetErrorResult(result);

            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/Logout
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        /// <summary>
        ///     OK PROBLEM JEST TAKI, ŻE NIE DZIAŁA XD.
        ///     W APLIKACJI MOBILNEJ TRZEBA PODLINKOWAĆ PO PROSTU BUTTON LUB ODNOŚNIK DO /Account/ForgotPassword i tyle.
        ///     Albo bawić się w robienie tego po API i przekierowywanie na widoki, ale wtedy token do zmiany hasła się nie zgadza
        ///     bo UserManagery są niezależne
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public IHttpActionResult ForgotPassword()
        {
            var homeUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return Redirect($"{homeUrl}/Account/ForgotPassword");
        }

        #endregion

        #region Helpers

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = new Url(Url.Link("ConfirmEmail", new {userId = userID, code}));
            await UserManager.SendEmailAsync(userID, subject,
                "Potwierdź swoje konto, klikając <a href=\"" + callbackUrl.Value + "\">tutaj</a>");
            return callbackUrl.ToString();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null) return InternalServerError();

            if (result.Succeeded) return null;
            if (result.Errors != null)
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);

            if (ModelState.IsValid)
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();

            return BadRequest(ModelState);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}