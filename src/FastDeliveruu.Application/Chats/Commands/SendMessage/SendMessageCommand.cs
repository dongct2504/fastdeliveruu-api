using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Dtos.ChatDtos;
using MediatR;

namespace FastDeliveruu.Application.Chats.Commands.SendMessage;

public class SendMessageCommand : IRequest<MessageDto>
{
    public Guid SenderId { get; set; }
    public SenderRecipientTypeEnum SenderType { get; set; }
    public Guid RecipientId { get; set; }
    public SenderRecipientTypeEnum RecipientType { get; set; }
    public string Content { get; set; } = null!;
}
