# DotNetCore.Validation

## Extensions

```cs
public static class Extensions
{
    public static void AddValidationService(this IServiceCollection services) { }

    public static Result Validation<T>(this IValidator<T> validator, T instance) { }
}
```
