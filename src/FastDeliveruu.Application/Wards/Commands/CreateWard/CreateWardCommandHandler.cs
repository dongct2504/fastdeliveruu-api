using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Wards.Commands.CreateWard;

public class CreateWardCommandHandler : IRequestHandler<CreateWardCommand, Result<WardDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWardCommandHandler> _logger;
    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateWardCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<WardDto>> Handle(CreateWardCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.DistrictId);
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        Ward? ward = await _unitOfWork.Wards
            .GetWithSpecAsync(new WardExistInDistrictSpecification(request.DistrictId, request.Name));
        if (ward != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardDuplicate));
        }

        ward = _mapper.Map<Ward>(request);
        ward.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Wards.Add(ward);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Wards, cancellationToken);

        return _mapper.Map<WardDto>(ward);
    }
}
