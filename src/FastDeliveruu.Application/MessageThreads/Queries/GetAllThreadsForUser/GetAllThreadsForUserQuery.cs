using FastDeliveruu.Application.Dtos.ChatDtos;
using MediatR;

namespace FastDeliveruu.Application.Chats.Queries.GetAllThreadsForUser;

public class GetAllThreadsForUserQuery : IRequest<List<MessageThreadDto>>
{
    public GetAllThreadsForUserQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
