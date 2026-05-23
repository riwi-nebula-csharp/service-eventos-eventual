using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Data;
using service_eventos_eventual.Services.Implementations;
using service_eventos_eventual.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<TeatroEventsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<IPlayService, PlayService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

