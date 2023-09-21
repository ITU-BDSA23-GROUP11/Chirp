
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Chirp.CSVDB;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();


var csvDatabase = new CsvDatabase("./../../data/chirp_db.csv");

app.MapGet("/cheeps", () =>
{
    var cheeps = csvDatabase.GetCheeps();
    return Results.Json(cheeps);
});

app.MapPost("/cheep", (Cheep newCheep) =>
{
    if (newCheep == null)
    {
        return Results.BadRequest("Invalid input");
    }

    csvDatabase.AddCheep(newCheep);
    return Results.Ok("Cheep added");
});

app.Run("http://localhost:5012");

//var cheep = await client.GetFromJsonAsync<Cheep>("cheeps");

