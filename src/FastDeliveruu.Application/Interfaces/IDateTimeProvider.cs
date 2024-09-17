namespace FastDeliveruu.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime VietnamDateTimeNow { get; }
}