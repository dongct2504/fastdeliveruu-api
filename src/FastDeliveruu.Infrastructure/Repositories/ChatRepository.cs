using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Infrastructure.Repositories;

public class ChatRepository : Repository<Chat>, IChatRepository
{
    public ChatRepository(FastDeliveruuDbContext context) : base(context)
    {
    }

    public void Update(Chat chat)
    {
        _dbContext.Update(chat);
    }
}
