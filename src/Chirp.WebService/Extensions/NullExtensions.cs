using System.Runtime.CompilerServices;

namespace Chirp.WebService.Extensions;

public static class NullExtensions
{
    public static T ThrowIfNull<T>(this T? argument, string? message = default, [CallerArgumentExpression("argument")] string? paramName = default)
        where T : struct
    {
        if (argument is null)
            throw new ArgumentNullException(paramName, message);
        return (T)argument;
    }
    
    public static void RunIfNotNull<T>(this T? argument, Action<T> function)
        where T : struct
    {
        if (argument is not null)
        {
            function((T)argument);
        }
    }
    
    public static TR RunIfNotNull<T, TR>(this T? argument, Func<T, TR> function, TR defaultValue)
        where T : struct
    {
        return argument is not null ? function((T)argument) : defaultValue;
    }
}