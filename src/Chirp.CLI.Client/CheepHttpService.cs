using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Chirp.CLI.Client;

public class CheepHttpService : ICheepService
{
    private readonly HttpClient _httpClient;

    public CheepHttpService()
    {
        
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5012");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    public async Task<Cheep[]> ReadCheeps()
    {
        var cheeps = await _httpClient.GetFromJsonAsync<Cheep[]>("/cheeps");

        if (cheeps != null) return cheeps;
        
        throw new HttpRequestException("Failed to read cheeps. Status code: {response.StatusCode}");
    }
    
    public async Task WriteCheep(Cheep cheep)
    {
        var response = await _httpClient.PostAsJsonAsync("/cheep", cheep);

        response.EnsureSuccessStatusCode();
    }
}