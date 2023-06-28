using DotNetCore.Results;
using FluentValidation;

namespace DotNetCore.Validation;

public static class Extensions
{
    public static Result Validation<T>(this IValidator<T> validator, T instance)
    {
        if (instance is null) return Result.Error(default);

        var result = validator.Validate(instance);

        return result.IsValid ? Result.Success() : Result.Error(result.ToString());
    }
}
