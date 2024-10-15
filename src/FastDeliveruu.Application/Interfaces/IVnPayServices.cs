using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IVnpayServices
{
    string CreatePaymentUrl(HttpContext httpContext, Order order);

    PaymentResponse PaymentExecute(IQueryCollection collection);
}