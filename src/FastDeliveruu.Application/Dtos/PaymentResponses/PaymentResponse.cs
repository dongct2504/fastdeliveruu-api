namespace FastDeliveruu.Application.Dtos.PaymentResponses;

public class PaymentResponse
{
    public bool IsSuccess { get; set; }

    public Guid OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string TransactionId { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string OrderDescription { get; set; } = null!;
}
