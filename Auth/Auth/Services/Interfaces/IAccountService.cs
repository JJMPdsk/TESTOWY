using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNet.Identity;

namespace Auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> Register(ApplicationUser user, string password);
        Task<IdentityResult> ConfirmEmail(string userId, string code);
    }
}