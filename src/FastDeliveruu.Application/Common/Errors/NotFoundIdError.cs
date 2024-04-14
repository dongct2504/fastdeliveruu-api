using FluentResults;

namespace FastDeliveruu.Application.Common.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
    {
        Message = message;
    }
}