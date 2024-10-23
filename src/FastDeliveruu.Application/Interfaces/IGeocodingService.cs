using FastDeliveruu.Domain.Entities.Identity;

namespace FastDeliveruu.Application.Interfaces;

public interface IGeocodingService
{
    Task<(double, double)?> ConvertToLatLongAsync(string fullAddress);
}
