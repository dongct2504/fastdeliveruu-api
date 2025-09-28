using System.ComponentModel;

namespace FastDeliveruu.Application.Common.Enums;

public enum PaymentStatusEnum
{
    [Description("Chờ thanh toán")]
    Pending = 1,

    [Description("Đang xử lý")]
    Processing,

    [Description("Đã hủy")]
    Cancelled,

    [Description("Thanh toán thất bại")]
    Failed,

    [Description("Đã thanh toán")]
    Approved,

    [Description("Đã giao hàng")]
    Shipped,

    [Description("Đã hoàn tiền")]
    Refunded,

    [Description("Thanh toán chậm")]
    DelayedPayment
}
