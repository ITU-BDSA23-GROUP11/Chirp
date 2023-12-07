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
    
    // With returning value
    public static TR? RunIfNotNull<T, TR>(this T? argument, Func<T, TR> function)
        where T : struct
        where TR: struct
    {
        if (argument is not null)
        {
            return function((T)argument);
        }

        return null;
    }
    
    // With returning value and default value
    public static TR RunIfNotNull<T, TR>(this T? argument, Func<T, TR> function, TR ifNullValue)
        where T : struct
        where TR: struct
    {
        if (argument is not null)
        {
            return function((T)argument);
        }
        return ifNullValue;
    }
    
    // With default action if arg is null
    public static void RunIfNotNull<T>(this T? argument, Action<T> function, Action ifNullAction)
        where T : struct
    {
        if (argument is not null)
        {
            function((T)argument);
        }
        else
        {
            ifNullAction();
        }
    }
}