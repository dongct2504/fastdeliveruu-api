using FastDeliveruu.Application.Common.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FastDeliveruu.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected ActionResult Problem(List<IError> errors)
    {
        var firstError = errors.First();

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
            case ValidationError:
                ModelStateDictionary modelStateDictionary = new ModelStateDictionary();

                foreach (ValidationError error in errors.First().Reasons)
                {
                    modelStateDictionary.AddModelError(error.PropertyName, error.Message);
                }

                return ValidationProblem(modelStateDictionary);
            default:
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    detail: firstError.Message);
        }
    }
}