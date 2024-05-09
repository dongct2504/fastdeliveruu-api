using FastDeliveruu.Application.Dtos.PaymentResponses;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IVnPayServices
{
    string CreatePaymentUrl(HttpContext httpContext, Order order);

    VnpayResponse PaymentExecute(IQueryCollection collection);
}