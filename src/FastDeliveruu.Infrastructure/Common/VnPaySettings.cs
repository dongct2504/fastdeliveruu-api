namespace FastDeliveruu.Infrastructure.Common;

public class VnpaySettings
{
    public const string SectionName = "Payment:Vnpay";

    public string Url { get; set; } = null!;

    public string Api { get; set; } = null!;

    public string TmnCode { get; set; } = null!;

    public string HashSecret { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string Command { get; set; } = null!;

    public string CurrCode { get; set; } = null!;

    public string Locale { get; set; } = null!;

    public string ReturnUrl { get; set; } = null!;
}