using System.Threading.Tasks;
using Data.Models;

namespace Data.Repositories.Interfaces
{
    public interface IUsersRepository : IRepository<ApplicationUser>
    {
        /// <summary>
        ///     Przypisuje token do użytkownika
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="token">Token</param>
        /// <returns></returns>
        Task<bool> AssignTokenToUserAsync(string userName, string token);

        /// <summary>
        ///     Zwraca obecny token użytkownika
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        Task<string> GetCurrentTokenAsync(string userName);

        /// <summary>
        ///     Zwraca użytkownika po nazwie użytkownika (username)
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        Task<ApplicationUser> GetUserByUserNameAsync(string userName);

        /// <summary>
        ///     Zwraca użytkownika po adresie email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetUserByEmailAsync(string email);

        /// <summary>
        ///     Zmienia claima FirstName użytkownika na nowy
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        Task<bool> ChangeUserFirstNameClaimAsync(string userId, string oldName, string newName);
    }
}