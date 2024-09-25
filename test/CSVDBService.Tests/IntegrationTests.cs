using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;

public class ChirpApiTests
{
    private const string BaseUrl = "http://localhost:5117";
    private readonly HttpClient _client;

    public ChirpApiTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [Fact]
    public async void GetCheeps_ReturnsOkAndListOfCheeps()
    {
        // Act
        var response = await _client.GetAsync("/cheeps");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cheeps = await response.Content.ReadFromJsonAsync<List<Cheep>>();
        Assert.NotNull(cheeps);
        Assert.True(cheeps.Count > 0); 
    }

    [Fact]
    public async void PostCheep_ReturnsOkAndStoresCheep()
    {
        // Arrange
        var newCheep = new Cheep("TestAuthor", "Test message", 1726660160);

        // Act
        var response = await _client.PostAsJsonAsync("/cheep", newCheep);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verifying if the cheep was added by making a GET request
        var cheeps = await _client.GetFromJsonAsync<List<Cheep>>("/cheeps");
        Assert.Contains(cheeps, c => c.Author == "TestAuthor" && c.Message == "Test message");
    }
}