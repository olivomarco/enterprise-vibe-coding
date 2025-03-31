using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WanderlustJournal.Services;
using WanderlustJournal.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SQLite database with EF Core
builder.Services.AddDbContext<JournalContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JournalContext") ?? 
                     "Data Source=WanderlustJournal.db"));

// Register JournalEntryService as scoped
builder.Services.AddScoped<JournalEntryService>();

// Register HttpClientFactory and configure the Nominatim API client
builder.Services.AddHttpClient("NominatimApi", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "WanderlustJournal/1.0");
    // Set a reasonable timeout
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register GeocodingService
builder.Services.AddScoped<GeocodingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Initialize the database using our migration helper
var logger = app.Services.GetRequiredService<ILogger<Program>>();
DatabaseMigrationHelper.EnsureDatabaseCreatedAndMigrated(app.Services, logger);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
