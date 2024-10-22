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

namespace FastDeliveruu.Application.Cities.Commands.CreateCity;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCityCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateCityCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateCityCommandHandler> logger,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var spec = new CityByNameSpecification(request.Name);
        City? city = await _unitOfWork.Cities.GetWithSpecAsync(spec, true);
        if (city != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CityDuplicate));
        }

        city = _mapper.Map<City>(request);
        city.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Cities.Add(city);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CityDto>(city);
    }
}
