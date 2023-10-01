using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Chirp.Utilities.Models;

namespace Chirp.CLI.Client;

public class CheepHttpService : ICheepService
{
    private static CheepHttpService? _instance;
    private readonly HttpClient _httpClient;

    public static CheepHttpService GetInstance()
    {
        if (_instance == null)
        {
            _instance = new CheepHttpService();
        }

        return _instance;
    }

    private CheepHttpService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5174");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
    }
    
    public async Task<Cheep[]> ReadCheeps()
    {
        var cheeps = await _httpClient.GetFromJsonAsync<Cheep[]>("/cheeps");

        if (cheeps != null) return cheeps;
        
        throw new HttpRequestException("Failed to read cheeps. Status code: {response.StatusCode}");
    }
    
    public async Task WriteCheep(Cheep cheep)
    {
        var response = await  _httpClient.PostAsync(
            "/cheep",
            new StringContent(JsonSerializer.Serialize(cheep), Encoding.UTF8, "application/json")
        );
    
        response.EnsureSuccessStatusCode();
    }
}