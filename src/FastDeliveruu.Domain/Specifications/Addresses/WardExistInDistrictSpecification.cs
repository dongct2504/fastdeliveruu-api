﻿using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Addresses;

public class WardExistInDistrictSpecification : Specification<Ward>
{
    public WardExistInDistrictSpecification(int districtId, string wardName)
        : base(w => w.DistrictId == districtId && w.Name == wardName)
    {
    }
}