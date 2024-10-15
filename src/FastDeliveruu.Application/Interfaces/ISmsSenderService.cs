using Vonage.Messaging;

namespace FastDeliveruu.Application.Interfaces;

public interface ISmsSenderService
{
    Task<SendSmsResponse> SendSmsAsync(Guid userId, string to, string message);
}
