using FastDeliveruu.Application.Chats.Queries.GetAllThreadsForUser;
using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos.ChatDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.MessageThreads.Queries.GetAllThreadsForUser;

public class GetAllThreadsForUserQueryHandler : IRequestHandler<GetAllThreadsForUserQuery, List<MessageThreadDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllThreadsForUserQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<List<MessageThreadDto>> Handle(GetAllThreadsForUserQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Threads}-{request.UserId}";

        List<MessageThreadDto>? messageThreadDtosCache = await _cacheService
            .GetAsync<List<MessageThreadDto>>(key, cancellationToken);
        if (messageThreadDtosCache != null)
        {
            return messageThreadDtosCache;
        }

        List<MessageThreadDto> messageThreadDtos = await _dbContext.MessageThreads
            .Where(mt => mt.SenderShipperId == request.UserId || mt.SenderAppUserId == request.UserId)
            .AsNoTracking()
            .ProjectToType<MessageThreadDto>()
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(key, messageThreadDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return messageThreadDtos;
    }
}
