using FastDeliveruu.Application.Dtos.PaymentResponses;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FastDeliveruu.Application.Common.Helpers;

public static class Utils
{
    public static string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }

    public static string GetIpAddress(HttpContext context)
    {
        var ipAddress = "127.0.0.1";
        try
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;

            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.IsIPv4MappedToIPv6)
                {
                    remoteIpAddress = remoteIpAddress.MapToIPv4();
                }

                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = Dns.GetHostEntry(remoteIpAddress)
                        .AddressList
                        .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                }

                if (remoteIpAddress != null)
                {
                    ipAddress = remoteIpAddress.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            return "Invalid IP:" + ex.Message;
        }

        return ipAddress;
    }

    public static string CreateResponsePaymentUrl(string redirectUrl, PaymentResponse paymentResponse)
    {
        UriBuilder uriBuilder = new UriBuilder(redirectUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query["orderId"] = paymentResponse.OrderId.ToString();
        query["isSuccess"] = paymentResponse.IsSuccess.ToString();
        query["totalAmount"] = paymentResponse.TotalAmount.ToString();
        query["transactionId"] = paymentResponse.TransactionId;
        query["paymentMethod"] = paymentResponse.PaymentMethod.ToString();
        query["orderDescription"] = paymentResponse.OrderDescription;

        if (paymentResponse is VnpayResponse)
        {
            VnpayResponse vnpayResponse = (VnpayResponse)paymentResponse;
            query["vnpayResponseCode"] = vnpayResponse.VnpayResponseCode;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    public static string GetGroupName(Guid calledId, Guid otherId)
    {
        bool stringCompare = string.CompareOrdinal(calledId.ToString(), otherId.ToString()) > 0;
        return stringCompare ? $"{calledId}-{otherId}" : $"{otherId}-{calledId}";
    }

    public static string GenerateOtpCode()
    {
        Random random = new Random();
        int otp = random.Next(100000, 999999);
        return otp.ToString();
    }
}

