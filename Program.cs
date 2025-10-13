using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString =
    builder.Configuration.GetConnectionString("SchoolContext") ?? "Data Source=school.db";

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddRazorPages();

builder.Services.AddHealthChecks();

var app = builder.Build();

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

app.MapHealthChecks("/healthz");

app.Run();
