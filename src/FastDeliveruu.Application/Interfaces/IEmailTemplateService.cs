using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IEmailTemplateService
{
    string GenerateEmailConfirmationBody(string userName, string confirmationLink);
    string GenerateResetPasswordBody(string userName, string resetPasswordLink);
    string GenerateOrderEmailBody(AppUser user, Order order, OrderStatusEnum status, PaymentMethodsEnum paymentMethod, PaymentStatusEnum paymentStatus);
}
