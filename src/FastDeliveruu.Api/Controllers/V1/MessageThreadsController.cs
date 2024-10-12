using Asp.Versioning;
using FastDeliveruu.Application.Chats.Queries.GetAllThreadsForUser;
using FastDeliveruu.Application.Dtos.ChatDtos;
using FastDeliveruu.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/message-threads")]
public class MessageThreadsController : ApiController
{
    private readonly IMediator _mediator;

    public MessageThreadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MessageThreadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllThreadsForUser()
    {
        GetAllThreadsForUserQuery query = new GetAllThreadsForUserQuery(User.GetCurrentUserId());
        List<MessageThreadDto> messageThreadDtos = await _mediator.Send(query);
        return Ok(messageThreadDtos);
    }
}
