using FastDeliveruu.Application.Common.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected ActionResult Problem(List<IError> errors)
    {
        var firstError = errors[0];

        switch (firstError)
        {
            case NotFoundError:
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    detail: firstError.Message);
            case BadRequestError:
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: firstError.Message);
            case DuplicateError:
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    detail: firstError.Message);
            case UnauthorizedError:
                return Problem(statusCode: StatusCodes.Status401Unauthorized,
                    detail: firstError.Message);
            default:
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    detail: firstError.Message);
        }
    }
}