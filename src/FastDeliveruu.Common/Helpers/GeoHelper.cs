namespace FastDeliveruu.Common.Helpers;

public static class GeoHelper
{
    // Tính khoảng cách dùng Haversine
    public static double CalculateDistance(
        double latFrom,
        double lonFrom,
        double latTo,
        double lonTo)
    {
        const double R = 6371;

        double dLat = DegreesToRadians(latTo - latFrom);
        double dLon = DegreesToRadians(lonTo - lonFrom);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(latFrom)) *
            Math.Cos(DegreesToRadians(latTo)) *
            Math.Sin(dLon / 2) *
            Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double DegreesToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
