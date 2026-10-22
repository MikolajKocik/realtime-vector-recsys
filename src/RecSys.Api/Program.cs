using Microsoft.EntityFrameworkCore;
using RecSys.Api.Endpoints;
using RecSys.Application;
using RecSys.Infrastructure;
using RecSys.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Register Minimal API Endpoints
app.MapTelemetryEndpoints();
app.MapRecommendationEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.CatalogItems.AnyAsync())
    {
        string seedSql = await File.ReadAllTextAsync("seed.sql");
        await dbContext.Database.ExecuteSqlRawAsync(seedSql);
    }
}

await app.RunAsync();

