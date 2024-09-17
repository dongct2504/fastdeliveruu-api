using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime VietnamDateTimeNow
    {
        get
        {
            string userTimeZoneId = "SE Asia Standard Time";
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(UtcNow, timeZoneInfo);
        }
    }
}