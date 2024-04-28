using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, PaginationResponse<GenreDto>>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GetAllGenresQueryHandler(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<GenreDto>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize
        };

        PaginationResponse<GenreDto> paginationResponse = new PaginationResponse<GenreDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            TotalRecords = await _genreRepository.GetCountAsync(),
            Items = _mapper.Map<IEnumerable<GenreDto>>(await _genreRepository.ListAllAsync(options))
        };

        return paginationResponse;
    }
}