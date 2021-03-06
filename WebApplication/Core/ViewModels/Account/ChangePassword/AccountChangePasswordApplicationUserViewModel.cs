﻿using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.ChangePassword
{
    public class AccountChangePasswordApplicationUserViewModel
    {
        [Required(ErrorMessage = "Pole Obecne hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Obecne hasło")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Pole Nowe hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "{0} musi składać się z co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Podane hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }
    }
}