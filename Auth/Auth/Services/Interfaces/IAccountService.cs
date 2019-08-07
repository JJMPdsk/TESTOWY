using System.Threading.Tasks;
using Auth.Models;
using Auth.ViewModels.Account;
using Microsoft.AspNet.Identity;

namespace Auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Login(LoginViewModel model);
        Task<bool> IsUserEmailConfirmedAsync(string userId);

        Task<ApplicationUser> FindByNameAsync(string name);
        Task<ApplicationUser> FindByEmailAsync(string email);

        Task<IdentityResult> Register(ApplicationUser user, string password);
        Task<IdentityResult> ConfirmEmail(string userId, string code);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword);
        Task<IdentityResult> ResetUserPasswordAsync(string userId, string code, string password);

        Task SendPasswordResetEmailConfirmationLinkAsync(string userId);
        void Logout();
    }
}