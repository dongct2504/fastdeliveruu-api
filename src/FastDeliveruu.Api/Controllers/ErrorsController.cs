using Asp.Versioning;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/errors")]
public class ErrorsController : ApiController
{
    private readonly FastDeliveruuDbContext _dbContext;

    public ErrorsController(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFoundRequest()
    {
        return Problem(Result.Fail(new NotFoundError("Not found.")).Errors);
    }

    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        return Problem(Result.Fail(new BadRequestError("Bad request.")).Errors);
    }

    [HttpGet("conflict")]
    public IActionResult GetConflictRequest()
    {
        return Problem(Result.Fail(new DuplicateError("Duplicate error.")).Errors);
    }

    [HttpGet("validation-error")]
    public IActionResult GetValidationErrorRequest()
    {
        ValidationError validationError = new ValidationError("", "Validation failures.");
        validationError.Reasons.Add(new ValidationError("propertyName1", "Error message 1."));
        validationError.Reasons.Add(new ValidationError("propertyName2", "Error message 2."));

        return Problem(Result.Fail(validationError).Errors);
    }

    [HttpGet("unauthorized")]
    [Authorize]
    public IActionResult GetAuthErrorRequest()
    {
        return Ok("Stuff");
    }

    [HttpGet("internal-server-error")]
    public IActionResult GetInternalServerErrorRequest()
    {
        var thing = _dbContext.MenuItems.Find(-1);

        var thingToReturn = thing!.ToString();

        return Ok(thingToReturn);
    }
}
