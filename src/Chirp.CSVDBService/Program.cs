using SimpleDb;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = CSVDatabase<Cheep>.Instance;

//Get request

app.MapGet("/cheeps", (int? limit) =>
{
    var cheeps = db.Read();

    if (limit.HasValue && limit.Value > 0)
    {
        cheeps = cheeps.Take(limit.Value).ToList();
    }

    return Results.Ok(cheeps);

});

// Post request

app.MapPost("/cheep", (Cheep cheep) =>
{
    db.Store(cheep);

    return Results.Ok("Cheep stored succes");

});


app.Run();


/* 

COMMAND I RAN TO TEST GET REQUEST (IN WINDOWS POWERSHELL):
curl http://localhost:5117/cheeps

/////////////////////////////////

COMMAND I RAN TO TEST POST REQUEST(IN WINDOWS POWERSHELL):
Invoke-WebRequest -Uri http://localhost:5117/cheep `
                   -Method POST `
                   -ContentType "application/json" `
                   -Body '{"Author": "asjo", "Message": "Testing Post request!", "Timestamp": 1684229348}'

*/