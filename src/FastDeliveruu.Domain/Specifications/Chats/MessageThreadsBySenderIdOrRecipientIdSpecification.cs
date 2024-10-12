using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Specifications.Chats;

public class MessageThreadsBySenderIdOrRecipientIdSpecification : Specification<MessageThread>
{
    public MessageThreadsBySenderIdOrRecipientIdSpecification(Guid senderId, Guid recipientId)
        : base(mt => 
            (mt.SenderShipperId == senderId && mt.RecipientShipperId == recipientId) ||
            (mt.SenderShipperId == senderId && mt.RecipientAppUserId == recipientId) ||
            (mt.SenderAppUserId == senderId && mt.RecipientAppUserId == recipientId) ||
            (mt.SenderAppUserId == senderId && mt.RecipientShipperId == recipientId))
    {
    }
}
