using System.Runtime.CompilerServices;

namespace Chirp.Core.Extensions;

public static class NullExtensions
{
    public static T ThrowIfNull<T>(this T? argument, string? message = default, [CallerArgumentExpression("argument")] string? paramName = default)
        where T : struct
    {
        if (argument is null)
            throw new ArgumentNullException(paramName, message);
        return (T)argument;
    }
}