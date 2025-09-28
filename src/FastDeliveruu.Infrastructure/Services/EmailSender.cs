using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Infrastructure.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FastDeliveruu.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailOptions _emailOptions;

    public EmailSender(IOptions<EmailOptions> options)
    {
        _emailOptions = options.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string messageBody)
    {
        string senderEmail = _emailOptions.SenderEmail;
        string password = _emailOptions.Password;

        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Cay Trieu Dong", senderEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = messageBody };

        using (SmtpClient client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}