using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Seeders;
using service_eventos_eventual.Services.Implementations;
using service_eventos_eventual.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<TeatroEventsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IPlayService, PlayService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TeatroEventsDbContext>();
    await SeatSeeder.SeedAsync(context);
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

