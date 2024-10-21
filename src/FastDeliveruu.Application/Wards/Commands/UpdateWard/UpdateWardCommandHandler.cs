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
    private readonly IMapper _mapper;

    public UpdateWardCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateWardCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateWardCommand request, CancellationToken cancellationToken)
    {
        District? district = await _unitOfWork.Districts.GetAsync(request.DistrictId);
        if (district == null)
        {
            string message = "District not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Ward? ward = await _unitOfWork.Wards.GetAsync(request.Id);
        if (ward == null)
        {
            string message = "Ward not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _mapper.Map(request, ward);
        ward.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
