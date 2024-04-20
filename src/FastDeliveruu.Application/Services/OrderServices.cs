using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class OrderServices : IOrderServices
{
    private readonly IOrderRepository _orderRepository;

    public OrderServices(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersByUserIdAsynd(Guid userId)
    {
        QueryOptions<Order> options = new QueryOptions<Order>
        {
            Where = o => o.LocalUserId == userId
        };

        return await _orderRepository.ListAllAsync(options);
    }

    public async Task<Result<Order>> GetOrderByIdAsync(long id)
    {
        Order? order = await _orderRepository.GetAsync(id);
        if (order == null)
        {
            return Result.Fail<Order>(new NotFoundError("not found order."));
        }

        return order;
    }

    public async Task<Result<long>> CreateOrderAsync(Order order)
    {
        Order orderCreate = await _orderRepository.AddAsync(order);

        return orderCreate.OrderId;
    }

    public async Task<Result> UpdateOrderAsync(long id, Order order)
    {
        Order? isOrderExist = await _orderRepository.GetAsync(id);
        if (isOrderExist == null)
        {
            return Result.Fail(new NotFoundError("not found order."));
        }

        await _orderRepository.UpdateAsync(order);

        return Result.Ok();
    }

    public async Task<Result> DeleteOrderAsync(long id)
    {
        Order? isOrderExist = await _orderRepository.GetAsync(id);
        if (isOrderExist == null)
        {
            return Result.Fail(new NotFoundError("not found order."));
        }

        await _orderRepository.UpdateAsync(isOrderExist);

        return Result.Ok();
    }
}