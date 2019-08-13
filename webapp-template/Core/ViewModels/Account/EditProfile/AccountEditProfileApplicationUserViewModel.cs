using System;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.EditProfile
{
    public class AccountEditProfileApplicationUserViewModel
    {
        [Required]
        [StringLength(255)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Data urodzenia")]
        public DateTime? BirthDate { get; set; }
    }
}