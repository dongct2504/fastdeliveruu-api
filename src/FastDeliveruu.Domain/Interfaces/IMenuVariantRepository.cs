using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Interfaces;

public interface IMenuVariantRepository : IRepository<MenuVariant>
{
    void Update(MenuVariant menuVariant);
}
