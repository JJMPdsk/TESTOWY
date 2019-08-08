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
            CreateMap<AccountRegisterViewModel, ApplicationUser>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.EmailConfirmed, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.Ignore())
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumber, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(x => x.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(x => x.LockoutEndDateUtc, opt => opt.Ignore())
                .ForMember(x => x.LockoutEnabled, opt => opt.Ignore())
                .ForMember(x => x.AccessFailedCount, opt => opt.Ignore());
            CreateMap<ApplicationUser, AccountRegisterViewModel>();

            CreateMap<ApplicationUser, AccountEditProfileViewModel>();
            CreateMap<AccountEditProfileViewModel, ApplicationUser>();
        }
    }
}