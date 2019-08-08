using System.Threading.Tasks;
using Core.ViewModels.Account;
using Data.Models;
using Microsoft.AspNet.Identity;

namespace Core.Services.Interfaces
{
    public interface IAccountService
    {
        void Logout(string authType);

        Task SendPasswordResetEmailConfirmationLinkAsync(string userId);
        Task ForgotPasswordAsync(string userId);

        Task<bool> Login(LoginViewModel model);
        Task<bool> IsUserEmailConfirmedAsync(string userId);

        Task<ApplicationUser> FindUserByNameAsync(string name);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<ApplicationUser> FindUserByIdAsync(string userId);

        Task<IdentityResult> Register(ApplicationUser user, string password);
        Task<IdentityResult> ConfirmUserEmail(string userId, string code);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword);
        Task<IdentityResult> ResetUserPasswordAsync(string userId, string code, string password);
    }
}