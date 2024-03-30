using AutoMapper;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Api.Profiles;

public class FastDeliveruuProfile : Profile
{
    public FastDeliveruuProfile()
    {
        // Source -> Target
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<Genre, GenreCreateDto>().ReverseMap();
        CreateMap<Genre, GenreUpdateDto>().ReverseMap();
    }
}
