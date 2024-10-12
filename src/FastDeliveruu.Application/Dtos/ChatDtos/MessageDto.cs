namespace FastDeliveruu.Application.Dtos.ChatDtos;

public class MessageDto
{
    public Guid SenderId { get; set; }
    public string SenderUserName { get; set; } = null!;

    public Guid RecipientId { get; set; }
    public string RecipientUserName { get; set; } = null!;

    public string Content { get; set; } = null!;
    public DateTime? DateSent { get; set; }
    public DateTime? DateRead { get; set; }

    public string? SenderImageUrl { get; set; }
    public string? RecipientImageUrl { get; set; }
}
