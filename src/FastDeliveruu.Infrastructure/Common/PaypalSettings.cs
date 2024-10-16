namespace FastDeliveruu.Infrastructure.Common;

public class PaypalSettings
{
    public const string SectionName = "Payment:Paypal";

    public string AppId { get; set; } = null!;

    public string AppSecret { get; set; } = null!;

    public string Mode { get; set; } = null!;

    public string BaseUrl => Mode == "Live"
        ? "https://api-m.paypal.com"
        : "https://api-m.sandbox.paypal.com";
}
