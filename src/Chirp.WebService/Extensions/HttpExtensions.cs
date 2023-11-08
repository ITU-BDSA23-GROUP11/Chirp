namespace Chirp.WebService.Extensions;

public static class HttpExtensions
{
    public static string GetPathUrl(this HttpRequest request)
    {
        var origin = request.Headers["Origin"].ToString();
        var referer = request.Headers["Referer"].ToString();

        return referer.Replace(origin, "");
    }
}