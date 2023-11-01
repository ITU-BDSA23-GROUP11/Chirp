using System.Net;

namespace Chirp.WebService.Tests.Utilities;

public class HTTPUtility
{
    //A utility to test if there is a connection to and endpoint
    public static Boolean TestConnection(String url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                return response.IsSuccessStatusCode;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("A connection error happened during WebService testing");
            return false;
        }
    }
}