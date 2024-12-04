using Chirp.Core.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Load database connection via configuration
// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));

builder.Services.AddDefaultIdentity<Author>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpDBContext>();


builder.Services.AddRazorPages();
// Register the service
builder.Services.AddScoped<ICheepService, CheepService>();
// Register the Repository
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

builder.Services.AddAuthentication(options => { })
    .AddGitHub(o =>
    {
        // Use different secrets based on the environment
        if (builder.Environment.IsDevelopment())
        {
            o.ClientId = builder.Configuration["authentication:github:clientId:local"] ??
                         throw new ArgumentException("GitHub ClientId for local dev not provided.");
            o.ClientSecret = builder.Configuration["authentication:github:clientSecret:local"] ??
                             throw new ArgumentException("GitHub ClientSecret for local dev not provided.");
        }
        else
        {
            o.ClientId = builder.Configuration["authentication:github:clientId"] ??
                         Environment.GetEnvironmentVariable("GitHub__ClientId") ??
                         throw new ArgumentException("GitHub ClientId for CI not provided.");
            o.ClientSecret = builder.Configuration["authentication:github:clientSecret"] ??
                             Environment.GetEnvironmentVariable("GitHub__ClientSecret") ??
                             throw new ArgumentException("GitHub ClientSecret for CI not provided.");
        }

        o.CallbackPath = "/signin-github";

        o.Scope.Add("user:email");
        o.Scope.Add("read:user");
    });

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // Enable HTTPS on port 5001
        });
    });
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();

    // Apply any pending migrations
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }