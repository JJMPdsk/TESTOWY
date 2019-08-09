using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.ForgotPassword
{
    public class AccountForgotPasswordApplicationUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}