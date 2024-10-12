using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.Chats;

public class MessageThreadsBySender : Specification<MessageThread>
{
    public MessageThreadsBySender(Guid senderId)
        : base(mt => mt.SenderShipperId == senderId || mt.SenderAppUserId == senderId)
    {
    }
}
