namespace FastDeliveruu.Infrastructure.Common;

public class SmsSettings
{
    public const string SectionName = "SmsSettings";

    public string ApiKey { get; set; } = null!;

    public string ApiSecret { get; set; } = null!;
}
