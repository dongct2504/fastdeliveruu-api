using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetDeliveryMethods;

public class GetDeliveryMethodsQueryHandler : IRequestHandler<GetDeliveryMethodsQuery, List<DeliveryMethodDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetDeliveryMethodsQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<List<DeliveryMethodDto>> Handle(
        GetDeliveryMethodsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.DeliveryMethods}";

        List<DeliveryMethodDto>? deliveryMethodDtosCache = await _cacheService
            .GetAsync<List<DeliveryMethodDto>>(key, cancellationToken);
        if (deliveryMethodDtosCache != null)
        {
            return deliveryMethodDtosCache;
        }

        List<DeliveryMethodDto> deliveryMethodDtos = await _dbContext.DeliveryMethods
            .AsNoTracking()
            .ProjectToType<DeliveryMethodDto>()
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(key, deliveryMethodDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return deliveryMethodDtos;
    }
}
