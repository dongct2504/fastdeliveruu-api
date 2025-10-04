using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Infrastructure.Services;

public class MailNotificationService : IMailNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<MailNotificationService> _logger;

    public MailNotificationService(IEmailSender emailSender, IEmailTemplateService emailTemplateService, ILogger<MailNotificationService> logger)
    {
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(string userName, string email, string confirmationLink)
    {
        string subject = "Xác nhận email - FastDeliveruu";
        string body = _emailTemplateService.GenerateEmailConfirmationBody(userName, confirmationLink);

        await _emailSender.SendEmailAsync(email, subject, body);
    }

    public async Task SendResetPasswordEmailAsync(string userName, string email, string resetPasswordLink)
    {
        string subject = "Đặt lại mật khẩu - FastDeliveruu";
        string body = _emailTemplateService.GenerateResetPasswordBody(userName, resetPasswordLink);

        await _emailSender.SendEmailAsync(email, subject, body);
    }

    public async Task SendOrderNotificationAsync(AppUser user, Order order, OrderStatusEnum orderStatus, PaymentMethodsEnum paymentMethod, PaymentStatusEnum paymentStatus)
    {
        string subject = orderStatus switch
        {
            OrderStatusEnum.Pending => $"[FastDeliveruu] Xác nhận đơn hàng #{order.Id}",
            OrderStatusEnum.Cancelled => $"[FastDeliveruu] Đơn hàng #{order.Id} đã bị hủy",
            OrderStatusEnum.Success => $"[FastDeliveruu] Đơn hàng #{order.Id} đã giao dịch thành công",
            OrderStatusEnum.Failed => $"[FastDeliveruu] Thanh toán đơn hàng #{order.Id} thất bại",
            OrderStatusEnum.Refunded => $"[FastDeliveruu] Đơn hàng #{order.Id} đã được hoàn tiền",
            _ => $"[FastDeliveruu] Cập nhật đơn hàng #{order.Id}"
        };

        string body = _emailTemplateService.GenerateOrderEmailBody(
            user,
            order,
            orderStatus,
            paymentMethod,
            paymentStatus
        );

        try
        {
            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gửi mail thất bại cho Order {OrderId}", order.Id);
            throw;
        }
    }
}
