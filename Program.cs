using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// DbContext (SQLite). Falls back to local file if the connection string is missing.
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SchoolContext")
        ?? "Data Source=school.db"));

var app = builder.Build();

// ---- Seed the database from Data/seed.xml (idempotent) ----
using (var scope = app.Services.CreateScope())
{
    await SeedXml.EnsureSeededAsync(scope.ServiceProvider, app.Environment.ContentRootPath);
}
// ------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

await app.RunAsync();
