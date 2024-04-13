using FastDeliveruu.Application.Interfaces;

namespace FastDeliveruu.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}