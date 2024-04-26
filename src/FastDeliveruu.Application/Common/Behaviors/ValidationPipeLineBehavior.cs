using FastDeliveruu.Application.Common.Errors;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Common.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : class // Result<T>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationPipelineBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator == null)
        {
            return await next();
        }

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        ValidationError error = new ValidationError("", "Validation failures.");
        Log.Warning($"Validation failures: {request.GetType().Name} - {string.Join(", ", validationResult.Errors)} - {request}");

        foreach (var validationFailure in validationResult.Errors)
        {
            error.Reasons.Add(new ValidationError(validationFailure.PropertyName,
                validationFailure.ErrorMessage));
        }

        return (dynamic)Result.Fail(error); // somehow convert to Result<T>
    }
}