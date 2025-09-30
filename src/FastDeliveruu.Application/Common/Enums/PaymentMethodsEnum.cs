using System.ComponentModel;

namespace FastDeliveruu.Application.Common.Enums;

public enum PaymentMethodsEnum
{
    [Description("Tiền mặt")]
    Cash = 1,
    [Description("VnPay")]
    Vnpay = 2,
    [Description("PayPal")]
    Paypal = 3,
    [Description("Momo")]
    Momo = 4
}
