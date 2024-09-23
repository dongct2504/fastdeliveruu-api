using FastDeliveruu.Application.Common.Enums;

namespace FastDeliveruu.Application.Dtos.PaymentResponses;

public class PaymentResponse
{
    public bool IsSuccess { get; set; }

    public Guid OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string TransactionId { get; set; } = null!;

    public PaymentMethods PaymentMethod { get; set; }

    public string OrderDescription { get; set; } = null!;
}
