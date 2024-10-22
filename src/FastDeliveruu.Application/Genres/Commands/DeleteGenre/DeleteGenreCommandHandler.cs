using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Commands.DeleteGenre;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, Result>
{
    private readonly ILogger<DeleteGenreCommandHandler> _logger;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;

    public DeleteGenreCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteGenreCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(request.Id);
        if (genre == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.GenreNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.GenreNotFound));
        }

        _unitOfWork.Genres.Delete(genre);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}