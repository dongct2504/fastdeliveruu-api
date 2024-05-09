namespace FastDeliveruu.Application.Dtos.PaymentResponses;

public class VnpayResponse : PaymentResponse
{
    public string VnpayResponseCode { get; set; } = null!;

    public string Token { get; set; } = null!;
}