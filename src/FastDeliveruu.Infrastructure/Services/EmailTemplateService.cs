using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Extensions;

namespace FastDeliveruu.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    public string GenerateEmailConfirmationBody(string userName, string confirmationLink)
    {
        return $@"
        <html>
          <body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'>
            <div style='max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
              <h2 style='color:#4CAF50; text-align:center;'>Chào mừng bạn đến với FastDeliveruu!</h2>
              <p>Xin chào <b>{userName}</b>,</p>
              <p>Cảm ơn bạn đã đăng ký tài khoản tại <b>FastDeliveruu</b>.</p>
              <p>Vui lòng xác nhận email của bạn bằng cách nhấn nút bên dưới:</p>
              <div style='text-align:center; margin: 20px;'>
                <a href='{confirmationLink}' 
                   style='background-color:#4CAF50; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>
                   Xác nhận Email
                </a>
              </div>
              <p>Nếu bạn không tạo tài khoản, vui lòng bỏ qua email này.</p>
              <hr/>
              <p style='font-size:12px; color:#888;'>© {DateTime.UtcNow.Year} FastDeliveruu. Mọi quyền được bảo lưu.</p>
            </div>
          </body>
        </html>";
    }

    public string GenerateResetPasswordBody(string userName, string resetPasswordLink)
    {
        return $@"
        <html>
          <body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'>
            <div style='max-width: 600px; margin: auto; background: #fff; padding: 20px; 
                        border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
              <h2 style='color:#4CAF50; text-align:center;'>Đặt lại mật khẩu - FastDeliveruu!</h2>
              <p>Xin chào <b>{userName}</b>,</p>
              <p>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản <b>FastDeliveruu</b>.</p>
              <p>Vui lòng nhấn vào nút bên dưới để tiến hành thay đổi mật khẩu:</p>
              <div style='text-align:center; margin: 20px;'>
                <a href='{resetPasswordLink}' 
                   style='background-color:#4CAF50; color:white; padding:10px 20px; 
                          text-decoration:none; border-radius:5px;'>
                   Đặt lại mật khẩu
                </a>
              </div>
              <p>Nếu bạn không yêu cầu thay đổi mật khẩu, vui lòng bỏ qua email này.</p>
              <hr/>
              <p style='font-size:12px; color:#888;'>© {DateTime.UtcNow.Year} FastDeliveruu. Mọi quyền được bảo lưu.</p>
            </div>
          </body>
        </html>";
    }

    public string GenerateOrderEmailBody(AppUser user, Order order, OrderStatusEnum orderStatus, PaymentMethodsEnum paymentMethod, PaymentStatusEnum paymentStatus)
    {
        string statusText = orderStatus.GetDescription();
        string paymentText = paymentMethod.GetDescription();
        string paymentStatusText = paymentStatus.GetDescription();

        return $@"
        <html>
          <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color:#4CAF50;'>FastDeliveruu - Thông báo đơn hàng</h2>
            <p>Xin chào <b>{user.UserName}</b>,</p>
            <p>Trạng thái đơn hàng của bạn đã được cập nhật:</p>
            <table style='border-collapse: collapse; width: 100%;'>
              <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Mã đơn</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.Id}</td>
              </tr>
              <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Tổng tiền</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.TotalAmount:N0} VND</td>
              </tr>
              <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Phương thức thanh toán</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{paymentText}</td>
              </tr>
              <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Trạng thái đơn hàng</td>
                <td style='padding: 8px; border: 1px solid #ddd;'><b>{statusText}</b></td>
              </tr>
              <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Trạng thái thanh toán</td>
                <td style='padding: 8px; border: 1px solid #ddd;'><b>{paymentStatusText}</b></td>
              </tr>
            </table>
            <p style='margin-top:20px;'>Cảm ơn bạn đã tin dùng <b>FastDeliveruu!</b></p>
          </body>
        </html>";
    }
}
