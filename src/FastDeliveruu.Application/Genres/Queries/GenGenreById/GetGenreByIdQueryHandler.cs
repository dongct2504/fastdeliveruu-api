using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<GenreDetailDto>>
{
    private readonly ILogger<GetGenreByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetGenreByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetGenreByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<GenreDetailDto>> Handle(
        GetGenreByIdQuery request,
        CancellationToken cancellationToken)
    {
        GenreDetailDto? genreDetailDto = await _dbContext.Genres
            .Where(g => g.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<GenreDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (genreDetailDto == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.GenreNotFound} - {request}");
            return Result.Fail(new NotFoundError(ErrorMessageConstants.GenreNotFound));
        }

        return genreDetailDto;
    }
}