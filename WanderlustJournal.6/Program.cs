using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using WanderlustJournal.Services;
using WanderlustJournal.Data;
using WanderlustJournal.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SQLite database with EF Core
builder.Services.AddDbContext<JournalContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JournalContext") ?? 
                     "Data Source=WanderlustJournal.db"));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3; // Set to minimal requirements for demo purposes
})
    .AddEntityFrameworkStores<JournalContext>()
    .AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

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
try {
    // Use the async version for better reliability
    DatabaseMigrationHelper.EnsureDatabaseCreatedAndMigratedAsync(app.Services, logger).Wait();
}
catch (Exception ex) {
    logger.LogError(ex, "An error occurred during database initialization");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
