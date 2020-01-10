using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.Login
{
    public class AccountLoginApplicationUserViewModel
    {
        [Required(ErrorMessage = "Pole Login jest wymagane")]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}