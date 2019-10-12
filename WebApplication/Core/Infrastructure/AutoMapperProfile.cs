using AutoMapper;
using Core.ViewModels.Account.EditProfile;
using Core.ViewModels.Account.GetUserDetails;
using Core.ViewModels.Account.Register;
using Data.Models;

namespace Core.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            MapsForAccount();
        }

        /// <summary>
        /// Zawiera profile dla kontrolera Account
        /// </summary>
        private void MapsForAccount()
        {
            CreateMap<AccountRegisterApplicationUserViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, AccountRegisterApplicationUserViewModel>();

            CreateMap<ApplicationUser, AccountEditProfileApplicationUserViewModel>();
            CreateMap<AccountEditProfileApplicationUserViewModel, ApplicationUser>();

            CreateMap<ApplicationUser, AccountGetUserDetailsApplicationUserViewModel>();
            CreateMap<AccountGetUserDetailsApplicationUserViewModel, ApplicationUser>();
        }
    }
}