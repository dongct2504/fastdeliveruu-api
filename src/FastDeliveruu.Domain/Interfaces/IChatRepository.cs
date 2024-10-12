using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IChatRepository : IRepository<Chat>
{
    void Update(Chat chat);
}
