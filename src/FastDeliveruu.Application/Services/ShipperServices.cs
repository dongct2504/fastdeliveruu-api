using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class ShipperServices : IShipperService
{
    private readonly IShipperRepository _shipperRepository;

    public ShipperServices(IShipperRepository shipperRepository)
    {
        _shipperRepository = shipperRepository;
    }

    public async Task<IEnumerable<Shipper>> GetAllShippersAsync()
    {
        return await _shipperRepository.ListAllAsync();
    }

    public async Task<Result<Shipper>> GetShipperByIdAsync(Guid id)
    {
        Shipper? shipper = await _shipperRepository.GetAsync(id);
        if (shipper == null)
        {
            return Result.Fail<Shipper>(new NotFoundError("Not found any shipper."));
        }

        return shipper;
    }

    public async Task<Result<Shipper>> GetShipperWithOrdersByIdAsync(Guid id)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            SetIncludes = "Orders",
            Where = s => s.ShipperId == id
        };
        Shipper? shipper = await _shipperRepository.GetAsync(options);
        if (shipper == null)
        {
            return Result.Fail<Shipper>(new NotFoundError("Not found any shipper."));
        }

        return shipper;
    }

    public async Task<Result<Shipper>> GetShipperByUserNameAsync(string username)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            Where = s => s.UserName == username
        };
        Shipper? shipper = await _shipperRepository.GetAsync(options);
        if (shipper == null)
        {
            return Result.Fail<Shipper>(new NotFoundError("Not found any shipper."));
        }

        return shipper;
    }

    public async Task<Result<Shipper>> GetShipperByEmailAsync(string email)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            Where = s => s.Email == email
        };
        Shipper? shipper = await _shipperRepository.GetAsync(options);
        if (shipper == null)
        {
            return Result.Fail<Shipper>(new NotFoundError("Not found any shipper."));
        }

        return shipper;
    }

    public async Task<Result<Shipper>> GetNearestShipperAsync(string Address, string Ward,
        string District, string City)
    {
        IEnumerable<Shipper> shippers = await _shipperRepository.ListAllAsync();
        if (!shippers.Any())
        {
            return Result.Fail<Shipper>(new NotFoundError("not found any nearby shipper."));
        }

        Shipper? nearestShipper = null;

        nearestShipper = shippers.FirstOrDefault(s => s.Address == Address && s.Ward == Ward &&
            s.District == District && s.City == City);
        if (nearestShipper != null)
        {
            return nearestShipper;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.Ward == Ward && s.District == District &&
            s.City == City);
        if (nearestShipper != null)
        {
            return nearestShipper;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.District == District && s.City == City);
        if (nearestShipper != null)
        {
            return nearestShipper;
        }

        nearestShipper = shippers.FirstOrDefault(s => s.City == City);
        if (nearestShipper != null)
        {
            return nearestShipper;
        }

        return shippers.First();
    }

    public async Task<Result<Guid>> CreateShipperAsync(Shipper shipper)
    {
        QueryOptions<Shipper> options = new QueryOptions<Shipper>
        {
            Where = s => s.Cccd == shipper.Cccd
        };
        Shipper? isShipperExist = await _shipperRepository.GetAsync(options);
        if (isShipperExist != null)
        {
            return Result.Fail<Guid>(new DuplicateError("The request shipper is already exist."));
        }

        Shipper createdShipper = await _shipperRepository.AddAsync(shipper);

        return createdShipper.ShipperId;
    }

    public async Task<Result> UpdateShipperAsync(Guid id, Shipper shipper)
    {
        Shipper? isShipperExist = await _shipperRepository.GetAsync(id);
        if (isShipperExist == null)
        {
            return Result.Fail(new NotFoundError("Not found any shipper."));
        }

        await _shipperRepository.UpdateAsync(shipper);

        return Result.Ok();
    }

    public async Task<Result> DeleteShipperAsync(Guid id)
    {
        Shipper? shipper = await _shipperRepository.GetAsync(id);
        if (shipper == null)
        {
            return Result.Fail(new NotFoundError("Not found any shipper."));
        }

        await _shipperRepository.DeleteAsync(shipper);

        return Result.Ok();
    }
}