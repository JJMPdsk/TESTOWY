using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required] [Display(Name = "Login")] public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")] public bool RememberMe { get; set; }
    }
}