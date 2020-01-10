using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.ResetPassword
{
    public class AccountResetPasswordApplicationUserViewModel
    {
        [Required(ErrorMessage = "Pole Login jest wymagane")]
        [Display(Name = "Login")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Pole Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "{0} musi składać się z co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Podane hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}