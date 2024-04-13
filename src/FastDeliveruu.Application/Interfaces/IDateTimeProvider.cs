namespace FastDeliveruu.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}