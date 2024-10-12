using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Dtos.ChatDtos;
using MediatR;

namespace FastDeliveruu.Application.Chats.Queries.GetAllMessagesBetweenUsers;

public class GetAllMessagesBetweenUsersQuery : IRequest<List<MessageDto>>
{
    public GetAllMessagesBetweenUsersQuery(
        Guid senderId,
        Guid recipientId,
        SenderRecipientTypeEnum recipientType,
        SenderRecipientTypeEnum senderType)
    {
        SenderId = senderId;
        RecipientId = recipientId;
        RecipientType = recipientType;
        SenderType = senderType;
    }

    public Guid SenderId { get; }
    public Guid RecipientId { get; }
    public SenderRecipientTypeEnum RecipientType { get; }
    public SenderRecipientTypeEnum SenderType { get; }
}
