using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using FastDeliveruu.Infrastructure.Common;
using FastDeliveruu.Application.Common.Libraries.Vnpay;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Common.Enums;

namespace FastDeliveruu.Infrastructure.Services;

public class VnpayServices : IVnpayServices
{
    private readonly VnpaySettings _vnpaySettings;

    public VnpayServices(IOptions<VnpaySettings> vnPaySettingOptions)
    {
        _vnpaySettings = vnPaySettingOptions.Value;
    }

    public string CreatePaymentUrl(HttpContext httpContext, Order order)
    {
        VnpayLibrary vnpayLibrary = new VnpayLibrary();
        vnpayLibrary.AddRequestData("vnp_Version", _vnpaySettings.Version);
        vnpayLibrary.AddRequestData("vnp_Command", _vnpaySettings.Command);
        vnpayLibrary.AddRequestData("vnp_TmnCode", _vnpaySettings.TmnCode);
        vnpayLibrary.AddRequestData("vnp_Amount", ((int)(order.TotalAmount * 100)).ToString("F0"));

        vnpayLibrary.AddRequestData("vnp_CreateDate", order.OrderDate?.ToString("yyyyMMddHHmmss")
            ?? DateTime.Now.ToString("yyyyMMddHHmmss"));

        vnpayLibrary.AddRequestData("vnp_CurrCode", _vnpaySettings.CurrCode);
        vnpayLibrary.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
        vnpayLibrary.AddRequestData("vnp_Locale", _vnpaySettings.Locale);

        vnpayLibrary.AddRequestData("vnp_OrderInfo", order.OrderDescription ?? string.Empty);
        vnpayLibrary.AddRequestData("vnp_OrderType", "other");
        vnpayLibrary.AddRequestData("vnp_ReturnUrl", _vnpaySettings.ReturnUrl);

        vnpayLibrary.AddRequestData("vnp_TxnRef", order.Id.ToString());

        return vnpayLibrary.CreateRequestUrl(_vnpaySettings.Url, _vnpaySettings.HashSecret);
    }

    public PaymentResponse PaymentExecute(IQueryCollection collection)
    {
        VnpayLibrary vnPayLibrary = new VnpayLibrary();

        foreach (var (key, value) in collection)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPayLibrary.AddResponseData(key, value.ToString());
            }
        }

        Guid vnp_orderId = Guid.Parse(vnPayLibrary.GetResponseData("vnp_TxnRef"));
        string vnp_transactionId = Convert.ToString(vnPayLibrary.GetResponseData("vnp_TransactionNo"));
        string vnp_SecureHash = collection.FirstOrDefault(sc => sc.Key == "vnp_SecureHash").Value;
        string vnp_ResponseCode = vnPayLibrary.GetResponseData("vnp_ResponseCode");
        string vnp_OrderInfo = vnPayLibrary.GetResponseData("vnp_OrderInfo");
        decimal vnp_Amount = Convert.ToDecimal(vnPayLibrary.GetResponseData("vnp_Amount")) / 100;

        PaymentResponse paymentResponse = new PaymentResponse();

        bool checkSignature = vnPayLibrary.ValidateSignature(vnp_SecureHash, _vnpaySettings.HashSecret);
        if (!checkSignature)
        {
            paymentResponse.IsSuccess = false;
            return paymentResponse;
        }

        paymentResponse.IsSuccess = true;
        paymentResponse.OrderId = vnp_orderId;
        paymentResponse.TotalAmount = vnp_Amount;
        paymentResponse.TransactionId = vnp_transactionId;
        paymentResponse.PaymentMethod = PaymentMethodsEnum.Vnpay;
        paymentResponse.OrderDescription = vnp_OrderInfo;
        paymentResponse.VnpayToken = vnp_SecureHash;
        paymentResponse.VnpayResponseCode = vnp_ResponseCode;

        return paymentResponse;
    }
}