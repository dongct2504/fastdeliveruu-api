using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Wards.Commands.UpdateWard;

public class UpdateWardCommandHandler : IRequestHandler<UpdateWardCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateWardCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public UpdateWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateWardCommandHandler> logger,
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

    public async Task<Result> Handle(UpdateWardCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.DistrictId);
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        Ward? ward = await _unitOfWork.Wards.GetAsync(request.Id);
        if (ward == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WardNotFound));
        }

        _mapper.Map(request, ward);
        ward.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.Wards, cancellationToken);

        return Result.Ok();
    }
}
