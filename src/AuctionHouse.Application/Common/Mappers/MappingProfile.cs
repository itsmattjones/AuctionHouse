namespace AuctionHouse.Application.Common.Mappers;

using AuctionHouse.Application.Users.Queries.GetCurrentUser;
using AuctionHouse.Domain.Entities;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, CurrentUserDto>()
            .ForMember(u => u.Username, m => m.MapFrom(cu => cu.UserName))
            .ForMember(u => u.Email, m => m.MapFrom(cu => cu.Email))
            .ForMember(u => u.ProfileImageUrl, m => m.MapFrom(cu => cu.ProfileImageUrl));
    }
}