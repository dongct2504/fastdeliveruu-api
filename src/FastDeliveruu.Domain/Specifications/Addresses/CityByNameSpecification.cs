using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Addresses;

public class CityByNameSpecification : Specification<City>
{
    public CityByNameSpecification(string name)
        : base(c => c.Name == name)
    {
    }
}
