using System.Net;
using System.Net.Http.Headers;

namespace Chirp.CLI.Client;

public abstract class HttpService<T> : IWebDbService<T>
{
    protected static HttpService<T>? Instance;
    protected readonly HttpClient HttpClient;
    private readonly string _localHost = "http://localhost:5174";
    private readonly string _remoteHost = "https://bdsagroup11chirpremotedb.azurewebsites.net";

    protected HttpService()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Accept.Clear();
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

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
            HttpClient.BaseAddress = new Uri(_localHost);
            return;
        }
        
        if (remoteOk && mode != "local")
        {
            HttpClient.BaseAddress = new Uri(_remoteHost);
            return;
        }

        if (localOk)
        {
            HttpClient.BaseAddress = new Uri(_localHost);
            return;
        }

        if (remoteOk)
        {
            HttpClient.BaseAddress = new Uri(_remoteHost);
            return;
        }
        
        throw new InvalidOperationException($"Could not ping to given MODE ${mode} nor any of the backends");
    }

    public abstract Task<T[]> Read();

    public abstract Task Write(T model);
}