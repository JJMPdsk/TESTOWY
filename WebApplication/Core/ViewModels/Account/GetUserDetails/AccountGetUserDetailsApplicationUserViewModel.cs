using System;

namespace Core.ViewModels.Account.GetUserDetails
{
    public class AccountGetUserDetailsApplicationUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}