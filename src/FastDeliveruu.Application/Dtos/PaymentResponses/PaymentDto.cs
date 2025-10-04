using FastDeliveruu.Common.Enums;

namespace FastDeliveruu.Application.Dtos.PaymentResponses;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public PaymentMethodsEnum? PaymentMethod { get; set; }
}
