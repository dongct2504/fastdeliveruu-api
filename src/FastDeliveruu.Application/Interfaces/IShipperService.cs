using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IShipperService
{
    Task<IEnumerable<Shipper>> GetAllShippersAsync();

    Task<Result<Shipper>> GetShipperByIdAsync(Guid id);
    Task<Result<Shipper>> GetShipperByUserNameAsync(string username);
    Task<Result<Shipper>> GetShipperByEmailAsync(string email);
    Task<Result<Shipper>> GetShipperWithOrdersByIdAsync(Guid id);
    Task<Result<Shipper>> GetNearestShipperAsync(string Address, string Ward, string District, string City);

    Task<Result<Guid>> CreateShipperAsync(Shipper shipper);
    Task<Result> UpdateShipperAsync(Guid id, Shipper shipper);
    Task<Result> DeleteShipperAsync(Guid id);
}