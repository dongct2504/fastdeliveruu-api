using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Infrastructure.Common;
using Microsoft.Extensions.Options;
using OpenCage.Geocode;

namespace FastDeliveruu.Infrastructure.Services;

public class GeocodingService : IGeocodingService
{
    private readonly OpenCageSettings _openCageSettings;

    public GeocodingService(IOptions<OpenCageSettings> options)
    {
        _openCageSettings = options.Value;
    }

    public async Task<(double, double)?> ConvertToLatLongAsync(string fullAddress)
    {
        //GoogleLocationService locationService = new GoogleLocationService(apikey: _configuration["Google:ApiKey"]);
        //MapPoint point = locationService.GetLatLongFromAddress(fullAddress);

        //addressesCustomer.Latitude = (decimal?)point.Latitude;
        //addressesCustomer.Longitude = (decimal?)point.Longitude;

        Geocoder geocoder = new Geocoder(_openCageSettings.ApiKey);
        GeocoderResponse geocoderResponse = await geocoder.GeocodeAsync(fullAddress);

        if (geocoderResponse == null || geocoderResponse.Results == null || geocoderResponse.Results.Length == 0)
        {
            return null;
        }

        Location? mostAccurateLocation = null;

        foreach (Location? item in geocoderResponse.Results)
        {
            if (!string.IsNullOrEmpty(item.Components.Road))
            {
                mostAccurateLocation = item;
                break;
            }
        }

        if (mostAccurateLocation == null || mostAccurateLocation.Geometry == null)
        {
            return null;
        }

        return (mostAccurateLocation.Geometry.Latitude, mostAccurateLocation.Geometry.Longitude);
    }
}
