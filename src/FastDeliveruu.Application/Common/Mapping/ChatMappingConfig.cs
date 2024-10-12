using FastDeliveruu.Application.Dtos.ChatDtos;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using Mapster;

namespace FastDeliveruu.Application.Common.Mapping;

public class ChatMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MessageThread, MessageThreadDto>()
            .Map(dest => dest.SenderId, src => src.SenderShipperId != null 
                ? src.SenderShipperId
                : src.SenderAppUserId)
            .Map(dest => dest.RecipientId, src => src.RecipientShipperId != null
                ? src.RecipientShipperId
                : src.RecipientAppUserId)
            .Map(dest => dest.LatestMessage, src => src.Chats
                .OrderByDescending(c => c.DateSent)
                .Select(c => c.Content)
                .FirstOrDefault());
    }
}
