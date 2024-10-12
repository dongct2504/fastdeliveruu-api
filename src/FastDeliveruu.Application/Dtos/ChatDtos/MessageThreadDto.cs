using FastDeliveruu.Application.Common.Enums;

namespace FastDeliveruu.Application.Dtos.ChatDtos;

public class MessageThreadDto
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }

    public SenderRecipientTypeEnum RecipientType { get; set; }
    public SenderRecipientTypeEnum SenderType { get; set; }

    public string Title { get; set; } = null!;
    public string LatestMessage { get; set; } = null!;
}
