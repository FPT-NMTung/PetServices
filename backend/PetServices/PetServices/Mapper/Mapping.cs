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
            CreateMap<ProductCategory, ProductCategoryDTO>()
                .ForMember(dest => dest.ProCategoriesId, opt => opt.MapFrom(src => src.ProCategoriesId))
                .ForMember(dest => dest.ProCategoriesName, opt => opt.MapFrom(src => src.ProCategoriesName))
                .ForMember(dest => dest.Desciptions, opt => opt.MapFrom(src => src.Desciptions))
                .ForMember(dest => dest.Prictue, opt => opt.MapFrom(src => src.Prictue));
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
