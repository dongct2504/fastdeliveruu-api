using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Commands.DeleteMenuVariant;

public class DeleteMenuVariantCommandHandler : IRequestHandler<DeleteMenuVariantCommand, Result>
{
    private readonly IMenuVariantRepository _menuVariantRepository;
    private readonly ILogger<DeleteMenuVariantCommandHandler> _logger;

    public DeleteMenuVariantCommandHandler(
        IMenuVariantRepository menuVariantRepository,
        ILogger<DeleteMenuVariantCommandHandler> logger)
    {
        _menuVariantRepository = menuVariantRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuVariant? menuVariant = await _menuVariantRepository.GetAsync(request.Id);
        if (menuVariant == null)
        {
            string message = "MenuVariant does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _menuVariantRepository.DeleteAsync(menuVariant);
        return Result.Ok();
    }
}
