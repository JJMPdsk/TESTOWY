using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Core.Services.Interfaces;
using Core.ViewModels.Account;
using Core.ViewModels.Account.ChangePassword;
using Core.ViewModels.Account.EditProfile;
using Core.ViewModels.Account.Register;
using Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Constants = Core.Utilities.Constants;

namespace Core.Controllers.Api
{
    /// <summary>
    /// API dla zarządzania kontem użytkownika
    /// Logowanie znajduje się pod endpointem /Token
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        #region Helpers

        /// <summary>
        /// Metoda pomocnicza do zwracania pełniejszych informacji o błędzie
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
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

        #region REST API

        /// <summary>
        /// POST api/Account/Register
        /// Endpoint do rejestrowania użytkownika
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(AccountRegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _mapper.Map<AccountRegisterViewModel, ApplicationUser>(model);

            var result = await _accountService.Register(user, model.Password);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        /// <summary>
        /// GET api/Account/ConfirmEmail
        /// Endpoint do potwierdzania emaila użytkownika.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
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

            // Jeśli potwierdzenie adresu email się powiedzie, to przekieruj użytkownika pod wskazany adres
            // (użytkownik potwierdza email klikając link na swojej skrzynce mailowej, więc można go przekierować też na inną stronę)
            if (result.Succeeded) return Redirect($"{Constants.Home}/Account/ConfirmEmailApi");

            GetErrorResult(result);
            return BadRequest();
        }

        /// <summary>
        /// POST api/Account/EditProfile
        /// Endpoint do edycji profilu użytkownika.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("EditProfile")]
        public async Task<IHttpActionResult> EditProfile(AccountEditProfileViewModel model)
        {
            var user = await _accountService.FindUserByIdAsync(User.Identity.GetUserId());
            if (user == null) return null;

            _mapper.Map(model, user);

            var result = await _accountService.UpdateUserAsync(user);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }

        /// <summary>
        /// POST api/Account/ChangePassword
        /// Endpoint do zmiany hasła użytkownika
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(AccountChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result =
                await _accountService.ChangeUserPasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                    model.NewPassword);

            if (!result.Succeeded) return GetErrorResult(result);

            _accountService.Logout(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        /// <summary>
        /// Endpoint do wylogowywania użytkownika przez API (dla przeglądarek [ciasteczka])
        /// Aby wylogować się z aplikacji mobilnej należy usunąć header Authorize Bearer {token} z requestów
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            _accountService.Logout(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        #endregion
    }
}