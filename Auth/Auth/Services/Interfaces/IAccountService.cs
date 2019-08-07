using System.Threading.Tasks;
using Auth.Models;
using Auth.ViewModels.Account;
using Microsoft.AspNet.Identity;

namespace Auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> Register(ApplicationUser user, string password);
        Task<IdentityResult> ConfirmEmail(string userId, string code);
        Task<bool> Login(LoginViewModel model);
        Task<ApplicationUser> FindByNameAsync(string name);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    }
}