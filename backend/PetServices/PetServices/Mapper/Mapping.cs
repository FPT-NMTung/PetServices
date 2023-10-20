﻿using AutoMapper;
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
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.Desciptions, opt => opt.MapFrom(src => src.Desciptions))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Picture))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.SerCategoriesId, opt => opt.MapFrom(src => src.SerCategoriesId))
                .ForMember(dest => dest.SerCategoriesName, opt => opt.MapFrom(src => src.SerCategories.SerCategoriesName));


            CreateMap<Account, AccountByAdminDTO>()
            .ForMember(dest => dest.Stt, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                        src.PartnerInfoId != null ? src.PartnerInfo.FirstName + " " + src.PartnerInfo.LastName :
                        (src.UserInfoId != null ? src.UserInfo.FirstName + " " + src.UserInfo.LastName : "Null")))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                        src.PartnerInfoId != null ? src.PartnerInfo.Province + ", " + src.PartnerInfo.District
                        + ", " + src.PartnerInfo.Commune + ", " + src.PartnerInfo.Address :
                        (src.UserInfoId != null ? src.UserInfo.Province + ", " + src.UserInfo.District
                        + ", " + src.UserInfo.Commune + ", " + src.UserInfo.Address : "Null")))
            .ForMember(dest => dest.RoleName,
                        opt => opt.MapFrom(src => src.Role.RoleName ?? "Null"));
        }
    }
}
