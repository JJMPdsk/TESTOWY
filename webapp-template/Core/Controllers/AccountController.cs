using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Core.Services.Interfaces;
using Core.ViewModels.Account;
using Core.ViewModels.Account.ChangePassword;
using Core.ViewModels.Account.EditProfile;
using Core.ViewModels.Account.ForgotPassword;
using Core.ViewModels.Account.Login;
using Core.ViewModels.Account.Register;
using Core.ViewModels.Account.ResetPassword;
using Data.Models;
using Microsoft.AspNet.Identity;

namespace Core.Controllers
{

    /// <summary>
    /// Kontroler MVC do zarządzania kontem użytkownika.
    /// Dostęp autoryzowany chyba, że akcja stanowi inaczej.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// Serwis do kontrolera zawierający logikę biznesową.
        /// </summary>
        private readonly IAccountService _accountService;
        /// <summary>
        /// AutoMapper do mapowania DOM <---> ViewModel
        /// </summary>
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

        /// <summary>
        /// Pomocnicza metoda dodająca do ModelState błędy, które następnie będą renderowane w widoku.
        /// </summary>
        /// <param name="result"></param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        /// <summary>
        /// Metoda pomonicza przenosząca użytkownika do strony głównej.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region Actions

        /// <summary>
        /// Zwraca widok do rejestracji.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Metoda do rejestracji - przyjmuje ViewModel, mapuje go na użytkownika, którego następnie rejestruje.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountRegisterApplicationUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _mapper.Map<AccountRegisterApplicationUserViewModel, ApplicationUser>(model);

            var result = await _accountService.Register(user, model.Password);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Zwraca widok do logowania.
        /// </summary>
        /// <param name="returnUrl">Url, z którego zostaliśmy przekierowani do metody
        /// (np. gdy niezalogowani chcemy dostać się do autoryzowanego widoku - jesteśmy
        /// kierowani do ekranu logowania).</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Loguje użytkownika.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">Url, z którego zostaliśmy przekierowani do metody
        /// /// (np. gdy niezalogowani chcemy dostać się do autoryzowanego widoku - jesteśmy
        /// /// kierowani do ekranu logowania).</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AccountLoginApplicationUserViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var loginSucceeded = await _accountService.Login(model);

            // Jeśli pomyślnie zalogowano, wracamy do miejsca, z którego zostaliśmy wysłani do ekranu logowania
            if (loginSucceeded) return RedirectToLocal(returnUrl);

            ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło.");
            // Jeśli doszliśmy tutaj, to coś się nie powiodło - pokaż widok jeszcze raz
            return View(model);
        }

        /// <summary>
        /// Bierze obecnie zalogowanego użytkownika i wypełnia jego danymi formularz
        /// do edycji danych, który następnie zwraca do użytkownika.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            var user = await _accountService.FindUserByUserNameAsync(HttpContext.User.Identity.Name);
            var model = _mapper.Map<ApplicationUser, AccountEditProfileApplicationUserViewModel>(user);
            return View(model);
        }

        /// <summary>
        /// Wysyła polecenie edycji profilu użytkownika i nadpisanie edytowanych pól użytkownika (np. imię).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(AccountEditProfileApplicationUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByUserNameAsync(HttpContext.User.Identity.Name);

            _mapper.Map(model, user);

            var result = await _accountService.UpdateUserAsync(user);

            if (result.Succeeded)
                ViewBag.Message = "Profil został zaktualizowany";
            else
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisu");

            return View(model);
        }

        /// <summary>
        /// Zwraca formularz do zmiany hasła.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Wysyła polecenie serwisowi aby ten zmienił hasło dla obecnie zalogowanego użytkownika.
        /// Użytkownik po zmianie hasła jest wylogowywany i proszony o ponowne zalogowanie z użyciem nowego hasła.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(AccountChangePasswordApplicationUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByUserNameAsync(HttpContext.User.Identity.Name);
            var result = await _accountService.ChangeUserPasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
                return RedirectToAction("Login", "Account");

            AddErrors(result);

            return View(model);
        }

        /// <summary>
        /// Widok wyświetlany po kliknięciu w link aktywacyjny użytkownika, który zarejestrował się przez API.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmEmailApi()
        {
            return View();
        }

        /// <summary>
        /// Widok wyświetlany po kliknięciu w link aktywacyjny użytkownika,
        /// który zarejestrował się przez MVC,
        /// pod warunkiem, że potwierdzenie adresu email się powiodło.
        /// </summary>
        /// <param name="userId">id użytkownika</param>
        /// <param name="code">token do weryfikacji</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");
            var result = await _accountService.ConfirmUserEmail(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Widok dla przypomnienia hasła, w którym użytkownik wprowadza adres email,
        /// na który ma zostać wysłany link do resetowania hasła.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Wysyła polecenie, by sprawdzić czy użytkownik potwierdził email.
        /// Jeśli potwierdził, to wysyła na adres email link do zresetowania hasła.
        /// Jeśli nie potwierdził, to wysyłany jest nowy link aktywacyjny na email.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(AccountForgotPasswordApplicationUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByEmailAsync(model.Email);

            if (user == null)
                // Nie pokazuj, że użytkownika nie ma w bazie
                return View("ForgotPasswordConfirmation");

            bool isUserEmailConfirmed = await _accountService.IsUserEmailConfirmedAsync(user.Id);

            // W zależności od stanu potwierdzenia adresu użytkownika wyśle link aktywujący konto lub link do resetu hasła
            await _accountService.ForgotPasswordAsync(user.Id);

            return isUserEmailConfirmed
                ? (ActionResult) RedirectToAction("ForgotPasswordConfirmation", "Account")
                : View("PleaseConfirmEmail");
        }

        /// <summary>
        /// Widok potwierdzający zmianę hasła - informuje
        /// użytkownika, by udał się na swoją skrzynkę mailową
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Zwraca widok do resetowania hasła.
        /// </summary>
        /// <param name="code">token</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// Resetuje hasło użytkownika i ustawia nowe zadane przez użytkownika.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(AccountResetPasswordApplicationUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _accountService.FindUserByUserNameAsync(model.UserName);
            if (user == null)
                // Nie pokazuj, że użytkownika nie ma w bazie
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            var result = await _accountService.ResetUserPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Account");
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Widok potwierdzający zresetowanie hasła.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Wylogowuje użytkownika i przenosi go na ekran logowania.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _accountService.Logout(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}