using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Models;
using Auth.Services.Interfaces;
using Auth.UnitOfWork;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Auth.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUnitOfWork _unitOfWork;


        private readonly ApplicationUserManager _userManager;


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

        public AccountService(IUnitOfWork unitOfWork, ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, IAuthenticationManager authenticationManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _authenticationManager = authenticationManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; }

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

        public async Task<IdentityResult> ConfirmEmail(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(userId, code);

            return result.Succeeded ? IdentityResult.Success : result;
        }

        #endregion


        #region Private methods

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = new Uri(Constants.Home + "/Account/ConfirmEmail")
                .AddParameter("userId", userID)
                .AddParameter("code", code).ToString();
            await _userManager.SendEmailAsync(userID, subject,
                "Potwierdź swoje konto, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");

            return callbackUrl;
        }

        #endregion
    }
}