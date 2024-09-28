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
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<CreateWardCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result<WardDto>> Handle(CreateWardCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.DistrictId);
        if (district == null)
        {
            string message = "District not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        Ward? ward = await _unitOfWork.Wards
            .GetWithSpecAsync(new WardExistInDistrictSpecification(request.DistrictId, request.Name));
        if (ward != null)
        {
            string message = "Ward is already exist in city.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
        }

        ward = _mapper.Map<Ward>(request);
        ward.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Wards.Add(ward);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WardDto>(ward);
    }
}
