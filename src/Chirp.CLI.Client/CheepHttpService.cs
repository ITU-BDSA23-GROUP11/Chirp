using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Chirp.CLI.Client;

public class CheepHttpService<T> : HttpService<T>
{
    public static HttpService<T> GetInstance()
    {
        Instance ??= new CheepHttpService<T>();
        return Instance;
    }
    
    public override async Task<T[]> Read()
    {
        var cheeps = await HttpClient.GetFromJsonAsync<T[]>("/cheeps");

        if (cheeps != null) return cheeps;
        
        throw new HttpRequestException("Failed to read cheeps. Status code: {response.StatusCode}");
    }
    
    public override async Task Write(T cheep)
    {
        var response = await  HttpClient.PostAsync(
            "/cheep",
            new StringContent(JsonSerializer.Serialize(cheep), Encoding.UTF8, "application/json")
        );
    
        response.EnsureSuccessStatusCode();
    }
}