using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Districts.Commands.UpdateDistrict;

public class UpdateDistrictCommandHandler : IRequestHandler<UpdateDistrictCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDistrictCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateDistrictCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateDistrictCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateDistrictCommand request, CancellationToken cancellationToken)
    {
        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityNotFound));
        }

        District? district = await _unitOfWork.Districts.GetAsync(request.Id);
        if (district == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.DistrictNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.DistrictNotFound));
        }

        _mapper.Map(request, district);
        district.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
