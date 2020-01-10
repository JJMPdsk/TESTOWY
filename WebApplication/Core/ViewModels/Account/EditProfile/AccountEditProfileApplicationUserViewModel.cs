using System;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels.Account.EditProfile
{
    public class AccountEditProfileApplicationUserViewModel
    {
        [Required(ErrorMessage = "Pole Imię jest wymagane")]
        [StringLength(255)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko jest wymagane")]
        [StringLength(255)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Data urodzenia")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd\\/MMMM\\/yyyy}")]
        public DateTime? BirthDate { get; set; }
    }
}