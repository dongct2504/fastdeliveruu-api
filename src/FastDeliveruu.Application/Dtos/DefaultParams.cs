namespace FastDeliveruu.Application.Dtos;

public class DefaultParams
{
    public string Sort { get; set; } = string.Empty;

    public string Search { get; set; } = string.Empty;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public override string? ToString()
    {
        return $"{Sort}-{Search}-{PageNumber}-{PageSize}";
    }
}
