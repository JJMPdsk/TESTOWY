using System;
using System.ComponentModel.DataAnnotations;

namespace Auth.ViewModels.Account
{
    public class EditProfileViewModel
    {
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