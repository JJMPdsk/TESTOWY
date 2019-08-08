using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.ForgotPassword
{
    public class AccountForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}