using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Genres.Commands.CreateGenre;
using FastDeliveruu.Application.Genres.Commands.UpdateGenre;
using FastDeliveruu.Domain.Entities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class GenreMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Genre, GenreDto>();

        config.NewConfig<Genre, GenreDetailDto>()
            .Map(dest => dest.MenuItemDtos, src => src.MenuItems);

        config.NewConfig<CreateGenreCommand, Genre>();

        config.NewConfig<UpdateGenreCommand, Genre>();
    }
}