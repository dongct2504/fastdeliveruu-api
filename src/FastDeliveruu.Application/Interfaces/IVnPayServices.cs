using FastDeliveruu.Application.Dtos.VnPayResponses;
using FastDeliveruu.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Interfaces;

public interface IVnPayServices
{
    string CreatePaymentUrl(HttpContext httpContext, Order order);

    VnPayResponse PaymentExecute(IQueryCollection collection);
}