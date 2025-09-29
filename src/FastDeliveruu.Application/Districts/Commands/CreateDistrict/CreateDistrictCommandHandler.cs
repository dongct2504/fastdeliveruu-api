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

namespace FastDeliveruu.Application.Districts.Commands.CreateDistrict;

public class CreateDistrictCommandHandler : IRequestHandler<CreateDistrictCommand, Result<DistrictDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDistrictCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CreateDistrictCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateDistrictCommandHandler> logger,
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

    public async Task<Result<DistrictDto>> Handle(CreateDistrictCommand request, CancellationToken cancellationToken)
    {
        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }

        District? district = await _unitOfWork.Districts
            .GetWithSpecAsync(new DistrictExistInCitySpecification(request.CityId, request.Name), true);
        if (district != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictDuplicate));
        }

        district = _mapper.Map<District>(request);
        district.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Districts.Add(district);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Districts, cancellationToken);

        return _mapper.Map<DistrictDto>(district);
    }
}
