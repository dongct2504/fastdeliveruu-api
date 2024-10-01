using FastDeliveruu.Application.Interfaces;
using Serilog.Core;
using Serilog.Events;

namespace FastDeliveruu.Infrastructure.Services;

public class VietnamDateTimeEnricher : ILogEventEnricher
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public VietnamDateTimeEnricher(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        DateTime vietnamDateTime = _dateTimeProvider.VietnamDateTimeNow;
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("VietnamDateTime", vietnamDateTime));
    }
}
