using System.Net;
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
    private readonly string _localHost = "http://localhost:5174";
    private readonly string _remoteHost = "https://bdsagroup11chirpremotedb.azurewebsites.net";

    public static CheepHttpService GetInstance()
    {
        _instance ??= new CheepHttpService();
        return _instance;
    }

    private CheepHttpService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

        Task<HttpResponseMessage> localPing = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_localHost+"/ping")));
        Task<HttpResponseMessage> remotePing = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_remoteHost+"/ping")));
        
        var pings = Task.WhenAll(
            localPing,
            remotePing
        ).Result;

        bool localOk = pings[0].StatusCode == HttpStatusCode.OK;
        bool remoteOk = pings[1].StatusCode == HttpStatusCode.OK;
        
        var mode = Environment.GetEnvironmentVariable("SERVICES") ?? "";

        if (localOk && mode != "remote")
        {
            _httpClient.BaseAddress = new Uri(_localHost);
            return;
        }
        
        if (remoteOk && mode != "local")
        {
            _httpClient.BaseAddress = new Uri(_remoteHost);
            return;
        }

        if (localOk)
        {
            _httpClient.BaseAddress = new Uri(_localHost);
            return;
        }

        if (remoteOk)
        {
            _httpClient.BaseAddress = new Uri(_remoteHost);
            return;
        }
        
        throw new InvalidOperationException($"Could not ping to given MODE ${mode} nor any of the backends");
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