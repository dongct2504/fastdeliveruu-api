using FluentResults;

namespace FastDeliveruu.Application.Common.Errors;

public class BadRequestError : Error
{
    public BadRequestError(string message)
    {
        Message = message;
    }
}
