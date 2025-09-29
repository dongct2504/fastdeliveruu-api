using FastDeliveruu.Application.Common.Enums;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderParams : DefaultParams
{
    public TimeRangeEnum TimeRange { get; set; } = TimeRangeEnum.All;

    public override string ToString()
    {
        return $"{base.ToString()}-{TimeRange}";
    }
}
