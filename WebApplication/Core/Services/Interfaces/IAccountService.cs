using System.Threading.Tasks;
using Core.Services.Utilities;
using Core.ViewModels.Account.Login;
using Data.Models;
using Microsoft.AspNet.Identity;

namespace Core.Services.Interfaces
{
    public interface IAccountService : IService
    {
        /// <summary>
        ///     Metoda wylogowująca użytkownika.
        /// </summary>
        /// <param name="authType">Typ autoryzacji (np. token, cookie)</param>
        void Logout(string authType);

        /// <summary>
        ///     Metoda obsługująca "zapomniałem hasła".
        ///     Jeśli użytkownik ma potwierdzony e-mail, to wyśle na ten adres link do resetowania hasła.
        ///     Jeśli adres nie jest potwierdzony, zostanie wysłany nowy link aktywacyjny
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ForgotPasswordAsync(string userId);

        /// <summary>
        ///     Metoda logowania
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Zwraca informacje o powiedzeniu się logowania</returns>
        Task<string> LoginAsync(AccountLoginApplicationUserViewModel model);

        /// <summary>
        ///     Metoda sprawdzająca czy użytkownik potwierdził adres e-mail
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True, jeśli adres jest potwierdzony i false w przeciwnym razie</returns>
        Task<bool> IsUserEmailConfirmedAsync(string userId);

        /// <summary>
        ///     Znajduje i zwraca użytkownika po nazwie użytkownika
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ApplicationUser> FindUserByUserNameAsync(string userName);

        /// <summary>
        ///     Znajduje i zwraca użytkownika po adresie email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ApplicationUser> FindUserByEmailAsync(string email);

        /// <summary>
        ///     Znajduje i zwraca użytkownika po Id użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApplicationUser> FindUserByIdAsync(string userId);

        /// <summary>
        ///     Metoda rejestrująca użytkownika
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string roleName);

        /// <summary>
        ///     Metoda do potwierdzania adresu e-mail użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<IdentityResult> ConfirmUserEmailAsync(string userId, string code);

        /// <summary>
        ///     Metoda do aktualizowania informacji o użytkowniku
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        /// <summary>
        ///     Metoda do zmiany hasła użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword);

        /// <summary>
        ///     Metoda do resetowania hasła użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> ResetUserPasswordAsync(string userId, string code, string password);

        /// <summary>
        ///     Pobiera i zwraca token przypisany do użytkownika
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<string> GetCurrentTokenAsync(string userName);

        /// <summary>
        ///     Zmienia nazwę w navbarze przy edycji imienia.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse> ChangeNameInNavbarAsync(string userId, string oldName, string newName);
    }
}