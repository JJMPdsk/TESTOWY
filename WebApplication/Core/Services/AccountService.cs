using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Services.Interfaces;
using Core.Services.Utilities;
using Core.Utilities;
using Core.ViewModels.Account.Login;
using Data.Models;
using Data.UnitOfWork;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Constants = Core.Utilities.Constants;

namespace Core.Services
{
    /// <summary>
    ///     Serwis obsługujący zarządzanie użytkownikiem
    /// </summary>
    public class AccountService : Service, IAccountService, IDisposable
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationUserManager _userManager;

        private static bool _firstRun = true;

        public AccountService(IUnitOfWork unitOfWork, IAuthenticationManager authenticationManager,
            ApplicationUserManager userManager, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _authenticationManager = authenticationManager;
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            if (_firstRun) EnsureAdminExists(adminUsername: Constants.AdminUsername, adminEmail: "systems@codeteam.pl");
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string roleName)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;

            // domyślnie tworzymy użytkownika o roli{claimsie} "User"
            await _userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Role, RoleName.User));
            await _userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Role, roleName));
            await _userManager.AddClaimAsync(user.Id, new Claim("FirstName", user.FirstName));

            // jeśli w aplikacji mamy tabele odpowiadające rolom, możemy
            // w tym miejscu zdefiniować typ rejestrowanego użytkownika i dodać go do osobnej tabeli
            // switch (roleName)
            // {
            //     case RoleName.Tourist:
            //         await AddTouristAsync(user.Id);
            //         break;
            //     case RoleName.Guide:
            //         await AddGuideAsync(user.Id);
            //         break;
            //     default:
            //         await AddTouristAsync(user.Id);
            //         break;
            // }

            await SignInAsync(user, false);

            await GenerateAccessTokenAsync(user.UserName, password);

            // potwierdzenie email
            await SendEmailConfirmationTokenAsync(user.Id, $"Witamy w serwisie {Constants.AppName}!", roleName);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ConfirmUserEmailAsync(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(userId, code);
            return result.Succeeded ? IdentityResult.Success : result;
        }

        public async Task<string> LoginAsync(AccountLoginApplicationUserViewModel model)
        {
            var user = await _userManager.FindAsync(model.UserName, model.Password);
            if (user == null) return "Nieprawidłowa nazwa użytkownika lub hasło.";
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            await SignInAsync(user, model.RememberMe);
            var result = await GenerateAccessTokenAsync(model.UserName, model.Password);
            return result;
        }

        public async Task<bool> IsUserEmailConfirmedAsync(string userId)
        {
            return await _userManager.IsEmailConfirmedAsync(userId);
        }

        public async Task<ApplicationUser> FindUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
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
                await SendEmailConfirmationTokenAsync(userId, $"Witamy w serwisie {Constants.AppName}!", RoleName.User);
        }


        public async Task<IdentityResult> ResetUserPasswordAsync(string userId, string code, string password)
        {
            var result = await _userManager.ResetPasswordAsync(userId, code, password);
            return result;
        }

        public async Task<string> GetCurrentTokenAsync(string userName)
        {
            return await _tokenProvider.GetCurrentTokenAsync(userName);
        }

        public async Task<ServiceResponse> ChangeNameInNavbarAsync(string userId, string oldName, string newName)
        {
            var result = await _unitOfWork.UsersRepository.ChangeUserFirstNameClaimAsync(userId, oldName, newName);
            return result == true ? new ServiceResponse(ResponseType.Ok) : new ServiceResponse(ResponseType.Error, "Wystąpił błąd podczas zmiany nazwy w claimie");
        }

        public void Logout(string authType)
        {
            _authenticationManager.SignOut(authType);
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _userManager?.Dispose();
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        /// <summary>
        ///     Metoda do wygenerowania tokena i przypisania go do użytkownika (przy logowaniu)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<string> GenerateAccessTokenAsync(string userName, string password)
        {
            using (var client = new HttpClient())
            {
                // zrób call do api (pobierz token)
                client.BaseAddress = new Uri(Constants.Home);
                client.DefaultRequestHeaders.Clear();

                // definicja parametrów żądania
                var stringContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password")
                });

                // POST 
                var result = await client.PostAsync("/api/account/login", stringContent);
                if (!result.IsSuccessStatusCode) return await result.Content.ReadAsStringAsync();

                // weź token z odpowiedzi
                var responseDetails =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(await result.Content.ReadAsStringAsync());
                var token = responseDetails.FirstOrDefault().Value;

                // przypisz token do usera
                if (!await _unitOfWork.UsersRepository.AssignTokenToUserAsync(userName, token))
                    return "Bład podczas przypisywania tokena użytkownikowi";
                await UpdateDatabaseAsync(_unitOfWork);
                return string.Empty;
            }
        }

        private void EnsureAdminExists(string adminUsername, string adminEmail)
        {
            _firstRun = false;
            var admin = _userManager.FindByName(adminUsername);
            if (admin != null) return;

            string adminFirstName = "Administrator";

            var adminAccount = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                FirstName = adminFirstName,
                LastName = Constants.AppName
            };

            var result = _userManager.Create(adminAccount, "Admin123!");
            if (!result.Succeeded) throw new Exception("Błąd podczas tworzenia konta administratora");

            _userManager.AddClaim(adminAccount.Id, new Claim(ClaimTypes.Role, RoleName.Administrator));
             _userManager.AddClaim(adminAccount.Id, new Claim("FirstName", adminFirstName));
            SendEmailConfirmationTokenAsync(adminAccount.Id, "Administratorze, potwierdź swoje konto", RoleName.Administrator).Wait();
        }

        private async Task SendPasswordResetEmailConfirmationLinkAsync(string userId)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(userId);
            var callbackUrl = new Uri(Constants.Home + "/konto/zresetuj-haslo").AddParameter("userId", userId)
                .AddParameter("code", code)
                .ToString();
            await _userManager.SendEmailAsync(userId, "Reset hasła",
                "Zresetuj hasło, klikając <a href=\"" + callbackUrl + "\">tutaj</a>");
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userId, string subject, string roleName)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = new Uri(Constants.Home + "/konto/potwierdzenie-email").AddParameter("userId", userId)
                .AddParameter("code", code)
                .ToString();

            // w tym miejscu możemy zdefiniować różne wiadomości email w zależności od roli użytkownika
            switch (roleName)
            {
                // case RoleName.Tourist:
                //     await _userManager.SendEmailAsync(userId, subject,
                //         "Witamy Przewodniku");
                //     break;
                // case RoleName.Guide:
                //     await _userManager.SendEmailAsync(userId, subject,
                //         "Witamy Przewodniku");
                //     break;
                default:
                    await _userManager.SendEmailAsync(userId, subject,
                        $"Witamy w serwisie {Constants.AppName}!<br/><br/>" +
                        "Potwierdź swój adres email, klikając <a href=\"" + callbackUrl + "\">tutaj</a>. Dzięki " +
                        "temu uzyskasz dostęp do funkcji przypominania hasła.");
                    break;
            }

            return callbackUrl;
        }
    }
}