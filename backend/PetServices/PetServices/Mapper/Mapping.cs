using AutoMapper;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;

namespace PetServices.Mapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserInfo, UserInfoDTO>()
                .ReverseMap();

            CreateMap<PartnerInfo, PartnerInfoDTO>()
                .ReverseMap();

            CreateMap<Account, AccountInfo>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.PartnerInfo,
                    opt => opt.MapFrom(src => src.PartnerInfo))
                .ReverseMap();

            CreateMap<Account, AccountDTO>()
                .ForMember(des => des.AccountId,
                            act => act.MapFrom(src => src.AccountId))
                .ForMember(des => des.Email,
                            act => act.MapFrom(src => src.Email))
                .ForMember(des => des.Password,
                            act => act.MapFrom(src => src.Password))
                .ForMember(des => des.Status,
                            act => act.MapFrom(src => src.Status))
                .ForMember(des => des.UserInfoId,
                            act => act.MapFrom(src => src.UserInfoId));

            CreateMap<ServiceCategory, ServiceCategoryDTO>()
                .ForMember(des => des.SerCategoriesId,
                            act => act.MapFrom(src => src.SerCategoriesId))
                .ForMember(des => des.SerCategoriesName,
                            act => act.MapFrom(src => src.SerCategoriesName))
                .ForMember(des => des.Desciptions,
                            act => act.MapFrom(src => src.Desciptions))
                .ForMember(des => des.Picture,
                            act => act.MapFrom(src => src.Picture));

            CreateMap<Service, ServiceDTO>()
                .ForMember(des => des.ServiceId,
                            act => act.MapFrom(src => src.ServiceId))
                .ForMember(des => des.ServiceName,
                            act => act.MapFrom(src => src.ServiceName))
                .ForMember(des => des.Desciptions,
                            act => act.MapFrom(src => src.Desciptions))
                .ForMember(des => des.Picture,
                            act => act.MapFrom(src => src.Picture))
                .ForMember(des => des.Price,
                            act => act.MapFrom(src => src.Price))
                .ForMember(des => des.Status,
                            act => act.MapFrom(src => src.Status))
                .ForMember(des => des.SerCategoriesId,
                            act => act.MapFrom(src => src.SerCategoriesId));

            CreateMap<Account, AccountByAdminDTO>()
            .ForMember(dest => dest.Stt, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                        src.PartnerInfoId != null ? src.PartnerInfo.FirstName + " " + src.PartnerInfo.LastName :
                        (src.UserInfoId != null ? src.UserInfo.FirstName + " " + src.UserInfo.LastName : "Null")))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                        src.PartnerInfoId != null ? src.PartnerInfo.Province + ", " + src.PartnerInfo.District
                        + ", " + src.PartnerInfo.Commune + ", " + src.PartnerInfo.Address :
                        (src.UserInfoId != null ? src.UserInfo.Province + ", " + src.UserInfo.District
                        + ", " + src.UserInfo.Commune + ", " + src.UserInfo.Address : "Null")));

            CreateMap<Account,UpdateAccountDTO >()
                .ForMember(des => des.Email,
                            act => act.MapFrom(src => src.Email))
                .ForMember(des => des.RoleId,
                            act => act.MapFrom(src => src.RoleId))
                .ForMember(des => des.Status,
                            act => act.MapFrom(src => src.Status));
        }
    }
}
