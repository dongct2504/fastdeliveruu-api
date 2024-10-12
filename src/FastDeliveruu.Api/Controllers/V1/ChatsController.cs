using Asp.Versioning;
using FastDeliveruu.Application.Chats.Commands.SendMessage;
using FastDeliveruu.Application.Chats.Queries.GetAllMessagesBetweenUsers;
using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Dtos.ChatDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/chats")]
public class ChatsController : ApiController
{
    private readonly IMediator _mediator;

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMessagesBetweenUsers(
        Guid senderId,
        Guid recipientId,
        SenderRecipientTypeEnum recipientType,
        SenderRecipientTypeEnum senderType)
    {
        GetAllMessagesBetweenUsersQuery query = new GetAllMessagesBetweenUsersQuery(senderId, recipientId, recipientType, senderType);
        List<MessageDto> messageDtos = await _mediator.Send(query);
        return Ok(messageDtos);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendMessageCommand command)
    {
        MessageDto messageDto = await _mediator.Send(command);
        return Ok(messageDto);
    }
}
