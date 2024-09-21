using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IMenuItemRepository : IRepository<MenuItem>
{
    void Update(MenuItem menuItem);
}