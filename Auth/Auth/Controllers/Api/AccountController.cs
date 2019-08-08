using System;
using System.Threading.Tasks;
using System.Web.Http;
using Data.Models;
using Auth.Services.Interfaces;
using Auth.ViewModels.Account;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;

namespace Auth.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
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

        public AccountController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #endregion


        #region REST API

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _mapper.Map<RegisterViewModel, ApplicationUser>(model);

            var result = await _accountService.Register(user, model.Password);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        // GET api/Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [Route(Name = "ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "userId i code są wymagane");
                return BadRequest(ModelState);
            }

            var result = await _accountService.ConfirmUserEmail(userId, code);

            if (result.Succeeded) return Redirect($"{Constants.Home}/Account/ConfirmEmailApi");

            GetErrorResult(result);
            return BadRequest();
        }

        // POST api/Account/EditProfile
        [HttpPost]
        [Route("EditProfile")]
        public async Task<IHttpActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _accountService.FindUserByIdAsync(User.Identity.GetUserId());
            if (user == null) return null;

            _mapper.Map(model, user);

            var result = await _accountService.UpdateUserAsync(user);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result =
                await _accountService.ChangeUserPasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                    model.NewPassword);

            if (!result.Succeeded) return GetErrorResult(result);

            _accountService.Logout(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/Logout
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            _accountService.Logout(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        #endregion

        #region Helpers

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

        #endregion
    }
}