using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Shippers.Commands.DeleteShipper;

public class DeleteShipperCommandHandler : IRequestHandler<DeleteShipperCommand, Result>
{
    private readonly IShipperRepository _shipperRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteShipperCommandHandler(
        IShipperRepository shipperRepository,
        IFileStorageServices fileStorageServices)
    {
        _shipperRepository = shipperRepository;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result> Handle(DeleteShipperCommand request, CancellationToken cancellationToken)
    {
        Shipper? shipper = await _shipperRepository.GetAsync(request.Id);
        if (shipper == null)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

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

        await _shipperRepository.DeleteAsync(shipper);

        return Result.Ok();
    }
}