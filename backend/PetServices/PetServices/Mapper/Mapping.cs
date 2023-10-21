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

            CreateMap<RoomCategory, RoomCategoryDTO>()
                .ReverseMap();

            CreateMap<Room, RoomDTO>()
                .ForMember(dest => dest.RoomCategoriesName, opt => opt.MapFrom(src => src.RoomCategories.RoomCategoriesName))
                .ReverseMap();

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Desciption, opt => opt.MapFrom(src => src.Desciption))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Picture))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ProCategoriesId, opt => opt.MapFrom(src => src.ProCategoriesId))
                .ForMember(dest => dest.ProCategoriesName, opt => opt.MapFrom(src => src.ProCategories.ProCategoriesName));

        }
    }
}
