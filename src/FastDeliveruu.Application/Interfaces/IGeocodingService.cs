namespace FastDeliveruu.Application.Interfaces;

public interface IGeocodingService
{
    Task<(double, double)?> ConvertToLatLong(string fullAddress);
}
