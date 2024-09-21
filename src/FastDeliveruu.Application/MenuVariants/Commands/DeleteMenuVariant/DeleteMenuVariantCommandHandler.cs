using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Commands.DeleteMenuVariant;

public class DeleteMenuVariantCommandHandler : IRequestHandler<DeleteMenuVariantCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuVariantCommandHandler> _logger;

    public DeleteMenuVariantCommandHandler(
        ILogger<DeleteMenuVariantCommandHandler> logger,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuVariant? menuVariant = await _unitOfWork.MenuVariants.GetAsync(request.Id);
        if (menuVariant == null)
        {
            string message = "MenuVariant does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _unitOfWork.MenuVariants.Delete(menuVariant);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
