using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, PaginationResponse<GenreDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GetAllGenresQueryHandler(IGenreRepository genreRepository, IMapper mapper, ICacheService cacheService)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<PaginationResponse<GenreDto>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Genres}-{request.PageNumber}";

        PaginationResponse<GenreDto>? paginationResponseCache = await _cacheService
            .GetAsync<PaginationResponse<GenreDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize
        };

        PaginationResponse<GenreDto> paginationResponse = new PaginationResponse<GenreDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Items = _mapper.Map<IEnumerable<GenreDto>>(
                await _genreRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _genreRepository.GetCountAsync()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}