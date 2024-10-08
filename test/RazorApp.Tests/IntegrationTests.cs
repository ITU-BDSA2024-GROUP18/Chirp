using Chirp.Razor;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Razor.App.Tests;

//Used: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
//      https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent.readasstringasync?view=net-7.0
//      https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/test/integration-tests/7.x/IntegrationTestsSample/tests/RazorPagesProject.Tests/RazorPagesProject.Tests.csproj

public class ChirpApiTests : IClassFixture<WebApplicationFactory<Program>> {

    private readonly WebApplicationFactory<Program> _factory;

    //private const string BaseUrl = "https://bdsagroup18achirpremotedb.azurewebsites.net/";

    public ChirpApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async void GetRequest_GetCheepFromAuthor()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Helge");

        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var cheep = await response.Content.ReadAsStringAsync();
        Assert.Contains("Hello, BDSA students!", cheep);
    }

    [Fact]
    public async void GetRequest_GetCheepFromAuthor2()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Adrian");

        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var cheep = await response.Content.ReadAsStringAsync();
        Assert.Contains("Hej, velkommen til kurset.", cheep);
    }

    [Fact]
    public async void GetRequest_CorrectContentType()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.Equal("text/html; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
}