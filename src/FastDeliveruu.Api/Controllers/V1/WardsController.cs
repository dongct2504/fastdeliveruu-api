using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/wards")]
public class WardsController : ApiController
{
    private readonly IMediator _mediator;

    public WardsController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
