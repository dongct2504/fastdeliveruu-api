using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Shippers.Commands.UpdateShipper;

public class UpdateShipperCommandHandler : IRequestHandler<UpdateShipperCommand, Result>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public UpdateShipperCommandHandler(
        IShipperRepository shipperRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices)
    {
        _shipperRepository = shipperRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result> Handle(UpdateShipperCommand request, CancellationToken cancellationToken)
    {
        Shipper? shipper = await _shipperRepository.GetAsync(request.ShipperId);
        if (shipper == null)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, shipper);

        if (request.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(shipper.PublicId))
            {
                DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(shipper.PublicId);
                if (deletionResult.Error != null)
                {
                    string message = deletionResult.Error.Message;
                    Log.Warning($"{request.GetType().Name} - {message} - {request}");
                    return Result.Fail(new BadRequestError(message));
                }
            }

            UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(
                request.ImageFile, UploadPath.ShipperImageUploadPath);

            shipper.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            shipper.PublicId = uploadResult.PublicId;
        }
        shipper.UpdatedAt = DateTime.Now;

        await _shipperRepository.UpdateAsync(shipper);

        return Result.Ok();
    }
}