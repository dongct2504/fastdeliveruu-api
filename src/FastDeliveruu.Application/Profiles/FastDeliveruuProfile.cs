using AutoMapper;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
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

        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<MenuItem, MenuItemDetailDto>()
            .ForMember(dest => dest.GenreDto, opt => opt.MapFrom(src => src.Genre))
            .ForMember(dest => dest.RestaurantDto, opt => opt.MapFrom(src => src.Restaurant));
        CreateMap<MenuItem, MenuItemCreateDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemUpdateDto>().ReverseMap();

        CreateMap<LocalUser, LocalUserDto>();
        CreateMap<RegisterationRequestDto, LocalUser>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role ?? "Customer"));
        CreateMap<LocalUserUpdateDto, LocalUser>();

        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.MenuItemDto, opt => opt.MapFrom(src => src.MenuItem));
        CreateMap<ShoppingCart, ShoppingCartCreateDto>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartUpdateDto>().ReverseMap();

        CreateMap<Shipper, ShipperDto>();
        CreateMap<Shipper, ShipperCreateDto>().ReverseMap();
        CreateMap<Shipper, ShipperUpdateDto>().ReverseMap();

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.LocalUserDto, opt => opt.MapFrom(src => src.LocalUser))
            .ForMember(dest => dest.ShipperDto, opt => opt.MapFrom(src => src.Shipper));
        CreateMap<OrderCreateDto, Order>();
    }
}
