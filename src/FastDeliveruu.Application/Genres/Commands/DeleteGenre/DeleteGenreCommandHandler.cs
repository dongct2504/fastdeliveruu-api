using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteGenreCommandHandler> _logger;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;

    public DeleteGenreCommandHandler(ICacheService cacheService, IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteGenreCommandHandler> logger)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(request.Id);
        if (genre == null)
        {
            string message = "Genre not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _unitOfWork.Genres.Delete(genre);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{CacheConstants.Genre}-{request.Id}", cancellationToken);

        return Result.Ok();
    }
}