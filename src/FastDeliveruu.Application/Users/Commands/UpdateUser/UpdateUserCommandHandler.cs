using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Addresses;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ILogger<UpdateUserCommand> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IFileStorageServices fileStorageServices,
        IMapper mapper,
        UserManager<AppUser> userManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<UpdateUserCommand> logger,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.Users
            .Include(u => u.AddressesCustomers)
            .Where(u => u.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            string message = "User not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, user);

        if (request.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(user.PublicId))
            {
                DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(user.PublicId);
                if (deletionResult.Error != null)
                {
                    string message = deletionResult.Error.Message;
                    _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                    return Result.Fail(new BadRequestError(message));
                }
            }

            UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(
                request.ImageFile, UploadPath.UserImageUploadPath);

            user.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            user.PublicId = uploadResult.PublicId;
        }

        // handle addresses
        City? city = await _unitOfWork.Cities.GetAsync(request.CityId);
        if (city == null)
        {
            string message = "City not found";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        District? district = await _unitOfWork.Districts.GetWithSpecAsync(
            new DistrictExistInCitySpecification(request.CityId, request.DistrictId));
        if (district == null)
        {
            string message = "District not found";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        Ward? ward = await _unitOfWork.Wards.GetWithSpecAsync(
            new WardExistInDistrictSpecification(request.DistrictId, request.WardId));
        if (ward == null)
        {
            string message = "Ward not found";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        AddressesCustomer? customerAddress = user.AddressesCustomers
            .Where(ac => ac.IsPrimary)
            .FirstOrDefault();
        if (customerAddress == null)
        {
            customerAddress = new AddressesCustomer
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                Address = request.Address,
                CityId = city.Id,
                DistrictId = district.Id,
                WardId = ward.Id,
                IsPrimary = true,
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            };

            _unitOfWork.AddressesCustomers.Add(customerAddress);
        }
        else
        {
            customerAddress.Address = request.Address;
            customerAddress.CityId = city.Id;
            customerAddress.DistrictId = district.Id;
            customerAddress.WardId = ward.Id;
        }

        user.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _userManager.UpdateAsync(user);

        return Result.Ok();
    }
}