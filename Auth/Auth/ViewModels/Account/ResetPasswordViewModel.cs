﻿using System.ComponentModel.DataAnnotations;

namespace Auth.ViewModels.Account
{
    public class ResetPasswordViewModel
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

        public string Code { get; set; }
    }
}