﻿using System.Threading.Tasks;
using Data.Models;
using Auth.ViewModels.Account;
using Microsoft.AspNet.Identity;

namespace Auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task SendPasswordResetEmailConfirmationLinkAsync(string userId);
        void Logout(string authType);

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