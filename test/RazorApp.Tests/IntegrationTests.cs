using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RazorApp.Tests
{
    public class ChirpApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ChirpApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<ChirpDBContext>(options =>
                    {
                        options.UseSqlite("DataSource=:memory:"); 
                    });

                    var serviceProvider = services.BuildServiceProvider();

                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
                    dbContext.Database.OpenConnection(); 
                    dbContext.Database.EnsureCreated(); 
                });
            });
        }

        [Fact]
        public async Task GetRequest_GetCheepFromAuthor()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Helge");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var cheep = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hello, BDSA students!", cheep);
        }

        [Fact]
        public async Task GetRequest_GetCheepFromAuthor2()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Adrian");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var cheep = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hej, velkommen til kurset.", cheep);
        }

        [Fact]
        public async Task GetRequest_CorrectContentType()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }
    }
}