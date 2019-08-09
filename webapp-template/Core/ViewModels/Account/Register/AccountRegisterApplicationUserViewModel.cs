using System;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.Register
{
    public class AccountRegisterApplicationUserViewModel
    {
        [Required] [Display(Name = "Login")] public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi składać się z co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Podane hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")] public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Data urodzenia")] public DateTime? BirthDate { get; set; }
    }
}