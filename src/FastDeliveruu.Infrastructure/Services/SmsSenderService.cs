using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Infrastructure.Common;
using Microsoft.Extensions.Options;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace FastDeliveruu.Infrastructure.Services;

public class SmsSenderService : ISmsSenderService
{
    private readonly SmsSettings _smsSettings;

    public SmsSenderService(IOptions<SmsSettings> smsOptions)
    {
        _smsSettings = smsOptions.Value;
    }

    public async Task<SendSmsResponse> SendSmsAsync(Guid userId, string to, string message)
    {
        // init the library
        var credentials = Credentials.FromApiKeyAndSecret(_smsSettings.ApiKey, _smsSettings.ApiSecret);
        var vonageClient = new VonageClient(credentials);

        SendSmsResponse response = await vonageClient.SmsClient.SendAnSmsAsync(new SendSmsRequest()
        {
            From = "FastDeliveruu",
            To = to,
            Text = message
        });

        return response;
    }
}
