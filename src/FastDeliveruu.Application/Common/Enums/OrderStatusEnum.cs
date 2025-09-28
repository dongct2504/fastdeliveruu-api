using System.ComponentModel;

namespace FastDeliveruu.Application.Common.Enums;

public enum OrderStatusEnum
{
    [Description("Chờ xác nhận")]
    Pending = 1,

    [Description("Đang xử lý")]
    Processing,

    [Description("Đã hủy")]
    Cancelled,

    [Description("Thất bại")]
    Failed,

    [Description("Thành công")]
    Success,

    [Description("Đã giao hàng")]
    Shipped,

    [Description("Đã hoàn tiền")]
    Refunded
}
