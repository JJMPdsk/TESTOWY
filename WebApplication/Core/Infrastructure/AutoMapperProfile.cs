using AutoMapper;
using Core.ViewModels.Account;
using Core.ViewModels.Account.EditProfile;
using Core.ViewModels.Account.Register;
using Data.Models;

namespace Core.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AccountRegisterApplicationUserViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, AccountRegisterApplicationUserViewModel>();

            CreateMap<ApplicationUser, AccountEditProfileApplicationUserViewModel>();
            CreateMap<AccountEditProfileApplicationUserViewModel, ApplicationUser>();
        }
    }
}