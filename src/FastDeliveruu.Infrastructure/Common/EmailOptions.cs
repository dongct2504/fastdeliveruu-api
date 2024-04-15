namespace FastDeliveruu.Infrastructure.Common;

public class EmailOptions
{
    public const string SectionName = "EmailOptions";

    public string SenderEmail { get; set; } = null!;

    public string Password { get; set; } = null!;
}