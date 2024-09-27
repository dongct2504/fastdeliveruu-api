using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Addresses;

public class DistrictExistInCitySpecification : Specification<District>
{
    public DistrictExistInCitySpecification(int cityId, string districtName)
        : base(d => d.Name == districtName && d.CityId == cityId)
    {
    }
}
