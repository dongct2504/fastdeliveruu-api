using FluentResults;

namespace FastDeliveruu.Application.Common.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError(string message)
    {
        Message = message;
    }
}