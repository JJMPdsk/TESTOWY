using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.ForgotPassword
{
    public class AccountForgotPasswordApplicationUserViewModel
    {
        [Required(ErrorMessage = "Pole Email jest wymagane")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}