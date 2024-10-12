using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IMessageThreadRepository : IRepository<MessageThread>
{
    void Update(MessageThread messageThread);
}
