namespace FastDeliveruu.Application.Common.Enums;

public enum PaymentStatusEnum
{
    Pending = 1,
    Processing,
    Cancelled,
    Failed,
    Approved,
    Shipped,
    Refunded,
    DelayedPayment
}
