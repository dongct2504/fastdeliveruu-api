using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Restaurants;

public class RestaurantHoursExistInRestaurantSpecification : Specification<RestaurantHour>
{
    public RestaurantHoursExistInRestaurantSpecification(
        Guid restaurantId, string? weekenDay, DateTime? startTime, DateTime? endTime)
        : base(rh => rh.WeekenDay == weekenDay
            && rh.StartTime == startTime
            && rh.EndTime == endTime
            && rh.RestaurantId == restaurantId)
    {
    }
}
