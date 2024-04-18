using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface ILocalUserRepository : IRepository<LocalUser>
{
    Task UpdateAsync(LocalUser localUser);
}