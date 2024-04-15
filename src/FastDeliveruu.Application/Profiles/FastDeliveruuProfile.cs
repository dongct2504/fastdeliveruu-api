using AutoMapper;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Profiles;

public class FastDeliveruuProfile : Profile
{
    public FastDeliveruuProfile()
    {
        // Source -> Target
        CreateMap<Genre, GenreDto>();
        CreateMap<Genre, GenreDetailDto>()
            .ForMember(dest => dest.MenuItemDtos, opt => opt.MapFrom(src => src.MenuItems));
        CreateMap<Genre, GenreCreateDto>().ReverseMap();
        CreateMap<Genre, GenreUpdateDto>().ReverseMap();

        CreateMap<Restaurant, RestaurantDto>();
        CreateMap<Restaurant, RestaurantDetailDto>()
            .ForMember(dest => dest.MenuItemDtos, opt => opt.MapFrom(src => src.MenuItems));
        CreateMap<Restaurant, RestaurantCreateDto>().ReverseMap();
        CreateMap<Restaurant, RestaurantUpdateDto>().ReverseMap();

        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));
        CreateMap<MenuItem, MenuItemDetailDto>()
            .ForMember(dest => dest.GenreDto, opt => opt.MapFrom(src => src.Genre))
            .ForMember(dest => dest.RestaurantDto, opt => opt.MapFrom(src => src.Restaurant));
        CreateMap<MenuItem, MenuItemCreateDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemUpdateDto>().ReverseMap();

        CreateMap<LocalUser, LocalUserDto>();
        CreateMap<RegisterationRequestDto, LocalUser>();
        CreateMap<LocalUserUpdateDto, LocalUser>();

        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.MenuItemDto, opt => opt.MapFrom(src => src.MenuItem));
        CreateMap<ShoppingCart, ShoppingCartCreateDto>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartUpdateDto>().ReverseMap();
    }
}
