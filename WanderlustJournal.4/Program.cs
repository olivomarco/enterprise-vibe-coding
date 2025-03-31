using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SQLite database with EF Core
builder.Services.AddDbContext<WanderlustJournal.Data.JournalContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JournalContext") ?? 
                     "Data Source=WanderlustJournal.db"));

// Register JournalEntryService as scoped (instead of singleton)
builder.Services.AddScoped<WanderlustJournal.Services.JournalEntryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Initialize the database (ensure it's created and migrated)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WanderlustJournal.Data.JournalContext>();
    context.Database.EnsureCreated(); // Creates the database if it doesn't exist
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
