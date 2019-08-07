using Auth.Models;
using Auth.ViewModels.Account;
using AutoMapper;

namespace Auth.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
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
            CreateMap<ApplicationUser, RegisterViewModel>();

            CreateMap<ApplicationUser, EditProfileViewModel>();
            CreateMap<EditProfileViewModel, ApplicationUser>();
        }
    }
}