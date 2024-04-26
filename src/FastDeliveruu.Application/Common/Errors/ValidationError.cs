using FluentResults;

namespace FastDeliveruu.Application.Common.Errors;

public class ValidationError : Error
{
    public ValidationError(string propertyName, string message)
    {
        PropertyName = propertyName;
        Message = message;
    }

    public string PropertyName { get; }
}