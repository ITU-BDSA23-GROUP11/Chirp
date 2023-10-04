using Chirp.CSVDB;
using Chirp.Utilities.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Ping endpoint to ensure connection
app.MapGet("/ping", () => Results.Ok());

app.MapGet("/cheeps", () => CsvDatabase.GetInstance().GetCheeps());

app.MapPost("/cheep", (Cheep cheep) =>
{
    CsvDatabase.GetInstance().AddCheep(cheep);
    return Results.Ok("Cheep added");
});

app.Run();
