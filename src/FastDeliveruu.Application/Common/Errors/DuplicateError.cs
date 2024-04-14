using FluentResults;

namespace FastDeliveruu.Application.Common.Errors;

public class DuplicateError : Error
{
    public DuplicateError(string message)
    {
        Message = message;
    }
}