using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Libraries.VnPay;
using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using FastDeliveruu.Infrastructure.Common;

namespace FastDeliveruu.Infrastructure.Services;

public class VnPayServices : IVnPayServices
{
    private readonly VnPaySettings _vnPaySettings;

    public VnPayServices(IOptions<VnPaySettings> vnPaySettingOptions)
    {
        _vnPaySettings = vnPaySettingOptions.Value;
    }

    public string CreatePaymentUrl(HttpContext httpContext, Order order)
    {
        VnPayLibrary vnPayLibrary = new VnPayLibrary();
        vnPayLibrary.AddRequestData("vnp_Version", _vnPaySettings.Version);
        vnPayLibrary.AddRequestData("vnp_Command", _vnPaySettings.Command);
        vnPayLibrary.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
        vnPayLibrary.AddRequestData("vnp_Amount", (order.TotalAmount * 100).ToString());

        vnPayLibrary.AddRequestData("vnp_CreateDate", order.OrderDate?.ToString("yyyyMMddHHmmss")
            ?? DateTime.Now.ToString("yyyyMMddHHmmss"));

        vnPayLibrary.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
        vnPayLibrary.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
        vnPayLibrary.AddRequestData("vnp_Locale", _vnPaySettings.Locale);

        vnPayLibrary.AddRequestData("vnp_OrderInfo", order.OrderDescription ?? string.Empty);
        vnPayLibrary.AddRequestData("vnp_OrderType", "other");
        vnPayLibrary.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);

        vnPayLibrary.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

        return vnPayLibrary.CreateRequestUrl(_vnPaySettings.Url, _vnPaySettings.HashSecret);
    }

    public VnpayResponse PaymentExecute(IQueryCollection collection)
    {
        VnPayLibrary vnPayLibrary = new VnPayLibrary();

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

        VnpayResponse vnpayResponse = new VnpayResponse();

        bool checkSignature = vnPayLibrary.ValidateSignature(vnp_SecureHash, _vnPaySettings.HashSecret);
        if (!checkSignature)
        {
            vnpayResponse.IsSuccess = false;
            return vnpayResponse;
        }

        vnpayResponse.IsSuccess = true;
        vnpayResponse.OrderId = vnp_orderId;
        vnpayResponse.TotalAmount = vnp_Amount;
        vnpayResponse.TransactionId = vnp_transactionId;
        vnpayResponse.PaymentMethod = PaymentMethods.Vnpay;
        vnpayResponse.OrderDescription = vnp_OrderInfo;
        vnpayResponse.Token = vnp_SecureHash;
        vnpayResponse.VnpayResponseCode = vnp_ResponseCode;

        return vnpayResponse;
    }
}