using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Commands.UpdateGenre;

public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UpdateGenreCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateGenreCommandHandler(
        IMapper mapper,
        IFastDeliveruuUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<UpdateGenreCommandHandler> logger,
        ICacheService cacheService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(request.Id);
        if (genre == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.GenreNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.GenreNotFound));
        }

        _mapper.Map(request, genre);
        genre.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveAsync(CacheConstants.Genres, cancellationToken);

        return Result.Ok();
    }
}