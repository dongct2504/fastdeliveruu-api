using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class MessageThreadRepository : Repository<MessageThread>, IMessageThreadRepository
{
    public MessageThreadRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(MessageThread messageThread)
    {
        _dbContext.MessageThreads.Add(messageThread);
    }
}
