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
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<Genre, GenreCreateDto>().ReverseMap();
        CreateMap<Genre, GenreUpdateDto>().ReverseMap();

        CreateMap<Restaurant, RestaurantDto>().ReverseMap();
        CreateMap<Restaurant, RestaurantCreateDto>().ReverseMap();
        CreateMap<Restaurant, RestaurantUpdateDto>().ReverseMap();

        CreateMap<MenuItem, MenuItemDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemCreateDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemUpdateDto>().ReverseMap();

        CreateMap<LocalUser, LocalUserDto>().ReverseMap();

        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartCreateDto>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartUpdateDto>().ReverseMap();
    }
}
