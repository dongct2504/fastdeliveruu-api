using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Application.Interfaces;

public interface IMailNotificationService
{
    Task SendEmailConfirmationAsync(string userName, string email, string confirmationLink);
    Task SendResetPasswordEmailAsync(string userName, string email, string resetPasswordLink);
    Task SendOrderNotificationAsync(AppUser user, Order order, OrderStatusEnum status, PaymentMethodsEnum paymentMethod, PaymentStatusEnum paymentStatus);
}
