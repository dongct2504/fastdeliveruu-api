using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IOrderServices
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<IEnumerable<Order>> GetAllOrdersByUserIdAsynd(Guid userId);

    Task<Result<Order>> GetOrderByIdAsync(long id);

    Task<Result<long>> CreateOrderAsync(Order order);
    Task<Result> UpdateOrderAsync(long id, Order order);
    Task<Result> DeleteOrderAsync(long id);
}