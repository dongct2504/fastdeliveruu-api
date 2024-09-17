using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Specifications.MenuVariants;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariants.Commands.CreateMenuVariant;

public class CreateMenuVariantCommandHandler : IRequestHandler<CreateMenuVariantCommand, Result<MenuVariantDto>>
{
    private readonly IMenuVariantRepository _menuVariantRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateMenuVariantCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateMenuVariantCommandHandler(
        IMenuVariantRepository menuVariantRepository,
        IMapper mapper,
        ILogger<CreateMenuVariantCommandHandler> logger,
        IMenuItemRepository menuItemRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _menuVariantRepository = menuVariantRepository;
        _mapper = mapper;
        _logger = logger;
        _menuItemRepository = menuItemRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<MenuVariantDto>> Handle(CreateMenuVariantCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        var spec = new MenuVariantExistInMenuItemSpecification(request.MenuItemId, request.VarietyName);

        MenuVariant? menuVariant = await _menuVariantRepository.GetWithSpecAsync(spec);
        if (menuVariant != null)
        {
            string message = "MenuVariant is already exist in MenuItem.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
        }

        menuVariant = _mapper.Map<MenuVariant>(request);
        menuVariant.Id = Guid.NewGuid();
        menuVariant.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _menuVariantRepository.AddAsync(menuVariant);

        return _mapper.Map<MenuVariantDto>(menuVariant);
    }
}
