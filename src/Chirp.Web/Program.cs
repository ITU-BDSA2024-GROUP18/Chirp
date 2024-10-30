using Chirp.Razor;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Load database connection via configuration
// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));


builder.Services.AddRazorPages();
// Register the service
builder.Services.AddScoped<ICheepService, CheepService>();
// Register the Repository
builder.Services.AddScoped<ICheepRepository, CheepRepository>();


var app = builder.Build();


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

app.MapRazorPages();

app.Run();

public partial class Program { }