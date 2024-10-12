using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace FastDeliveruu.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wishlist")]
public class WishListController : ApiController
{
    private readonly IMediator _mediator;

    public WishListController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
