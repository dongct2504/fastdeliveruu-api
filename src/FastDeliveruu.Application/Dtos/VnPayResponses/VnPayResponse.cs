namespace FastDeliveruu.Application.Dtos.VnPayResponses;

public class VnPayResponse
{
    public bool IsSuccess { get; set; }

    public long OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string TransactionId { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string OrderDescription { get; set; } = null!;

    public string VnPayResponseCode { get; set; } = null!;

    public string Token { get; set; } = null!;
}