using FastDeliveruu.Application.Dtos.ChatDtos;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Chats.Queries.GetAllMessagesBetweenUsers;

public class GetAllMessagesBetweenUsersQueryHandler : IRequestHandler<GetAllMessagesBetweenUsersQuery, List<MessageDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllMessagesBetweenUsersQueryHandler(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MessageDto>> Handle(GetAllMessagesBetweenUsersQuery request, CancellationToken cancellationToken)
    {
        List<MessageThread> messageThreads = await _dbContext.MessageThreads
            .Where(m =>
                (m.SenderShipperId == request.SenderId && m.RecipientShipperId == request.RecipientId) ||
                (m.SenderShipperId == request.SenderId && m.RecipientAppUserId == request.RecipientId) ||
                (m.SenderAppUserId == request.SenderId && m.RecipientAppUserId == request.RecipientId) ||
                (m.SenderAppUserId == request.SenderId && m.RecipientShipperId == request.RecipientId) ||
                // vice versa
                (m.SenderShipperId == request.RecipientId && m.RecipientShipperId == request.SenderId) ||
                (m.SenderShipperId == request.RecipientId && m.RecipientAppUserId == request.SenderId) ||
                (m.SenderAppUserId == request.RecipientId && m.RecipientAppUserId == request.SenderId) ||
                (m.SenderAppUserId == request.RecipientId && m.RecipientShipperId == request.SenderId))
            .Include(m => m.Chats)
            .Include(m => m.SenderAppUser)
            .Include(m => m.RecipientAppUser)
            .Include(m => m.SenderShipper)
            .Include(m => m.RecipientShipper)
            .ToListAsync();

        List<Chat> chats = messageThreads
            .SelectMany(mt => mt.Chats)
            .OrderBy(m => m.DateSent)
            .ToList();

        List<MessageDto> messageDtos = chats
            .Select(c => new MessageDto
            {
                Content = c.Content,
                DateSent = c.DateSent,
                DateRead = c.DateRead,

                SenderId = c.MessageThread.SenderAppUser != null
                    ? c.MessageThread.SenderAppUserId ?? Guid.Empty
                    : c.MessageThread.SenderShipperId ?? Guid.Empty,

                SenderUserName = c.MessageThread.SenderAppUser != null
                    ? c.MessageThread.SenderAppUser.UserName
                    : c.MessageThread.SenderShipper.UserName,

                SenderImageUrl = c.MessageThread.SenderAppUser != null
                    ? c.MessageThread.SenderAppUser.ImageUrl
                    : c.MessageThread.SenderShipper.ImageUrl,

                RecipientId = c.MessageThread.RecipientAppUserId != null
                    ? c.MessageThread.RecipientAppUserId ?? Guid.Empty
                    : c.MessageThread.RecipientShipperId ?? Guid.Empty,

                RecipientUserName = c.MessageThread.RecipientAppUser != null
                    ? c.MessageThread.RecipientAppUser.UserName
                    : c.MessageThread.RecipientShipper.UserName,

                RecipientImageUrl = c.MessageThread.RecipientAppUser != null
                    ? c.MessageThread.RecipientAppUser.ImageUrl
                    : c.MessageThread.RecipientShipper.ImageUrl
            })
            .ToList();

        return messageDtos;
    }
}
