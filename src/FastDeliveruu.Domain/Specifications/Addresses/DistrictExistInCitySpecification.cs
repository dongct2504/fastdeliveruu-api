using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Addresses;

public class DistrictExistInCitySpecification : Specification<District>
{
    public DistrictExistInCitySpecification(int cityId, string districtName)
        : base(d => d.CityId == cityId && d.Name == districtName)
    {
    }

    public DistrictExistInCitySpecification(int cityId, int districtId)
        : base(d => d.CityId == cityId && d.Id == districtId)
    {
    }
}
