using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Services.Interfaces;
using Core.Utilities;
using Core.ViewModels.Account;
using Data.Models;
using Data.UnitOfWork;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Constants = Core.Utilities.Constants;

namespace Core.Services
{
    public class AccountService : IAccountService, IDisposable
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUnitOfWork _unitOfWork;


        private readonly ApplicationUserManager _userManager;


        // prawdopodobnie do wyrzucenia
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; }

        #region Constructors

        public AccountService()
        {
        }

        public AccountService(IUnitOfWork unitOfWork, IAuthenticationManager authenticationManager,
            ApplicationUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        // prawdopodobnie do wyrzucenia
        public AccountService(IUnitOfWork unitOfWork, ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, IAuthenticationManager authenticationManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _authenticationManager = authenticationManager;
            AccessTokenFormat = accessTokenFormat;
        }

        #endregion

        #region Public methods

        public async Task<IdentityResult> Register(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;

            // domyślnie tworzymy użytkownika o roli{claimsie} "User"
            await _userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Role, RoleName.User));
            await SignInAsync(user, false);

            // potwierdzenie email
            await SendEmailConfirmationTokenAsync(user.Id, "Potwierdź swoje konto");

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ConfirmUserEmail(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(userId, code);

            return result.Succeeded ? IdentityResult.Success : result;
        }

        public async Task<bool> Login(LoginViewModel model)
        {
            var user = await _userManager.FindAsync(model.UserName, model.Password);
            if (user == null) return false;
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            await SignInAsync(user, model.RememberMe);
            return true;
        }

        public async Task<bool> IsUserEmailConfirmedAsync(string userId)
        {
            return await _userManager.IsEmailConfirmedAsync(userId);
        }


        public async Task<ApplicationUser> FindUserByNameAsync(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return user;
        }

        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }


        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
            if (result.Succeeded) _authenticationManager.SignOut();
            return result;
        }

        /// <summary>
        ///     Wysyła na podanego przez użytkownika emaila link do resetowania hasła lub (jeśli konto nie jest aktywowane) link
        ///     aktywacyjny
        /// </summary>
        /// <param name="userId">Id użytkownika</param>
        /// <returns></returns>
        public async Task ForgotPasswordAsync(string userId)
        {
            if (await IsUserEmailConfirmedAsync(userId))
                await SendPasswordResetEmailConfirmationLinkAsync(userId);
            else
                await SendEmailConfirmationTokenAsync(userId, "Potwierdź swoje konto");
        }


        public async Task SendPasswordResetEmailConfirmationLinkAsync(string userId)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(userId);
            var callbackUrl = new Uri(Constants.Home + "/Account/ResetPassword")
                .AddParameter("userId", userId)
                .AddParameter("code", code).ToString();
            await _userManager.SendEmailAsync(userId, "Reset hasła",
                "Zresetuj hasło, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");
        }

        public async Task<IdentityResult> ResetUserPasswordAsync(string userId, string code, string password)
        {
            var result = await _userManager.ResetPasswordAsync(userId, code, password);
            return result;
        }

        public void Logout(string authType)
        {
            _authenticationManager.SignOut(authType);
        }

        #endregion


        #region Private methods

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userId, string subject)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = new Uri(Constants.Home + "/Account/ConfirmEmail")
                .AddParameter("userId", userId)
                .AddParameter("code", code).ToString();
            await _userManager.SendEmailAsync(userId, subject,
                "Potwierdź swoje konto, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");

            return callbackUrl;
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _userManager?.Dispose();
        }

        #endregion
    }
}