using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Seeders;
using service_eventos_eventual.Services.BackgroundServices;
using service_eventos_eventual.Services.Implementations;
using service_eventos_eventual.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            )
        };
    });

builder.Services.AddAuthorization();
// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<TeatroEventsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));;
});

builder.Services.AddScoped<IPlayService, PlayService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddHostedService<PerformanceStatusUpdaterService>();
builder.Services.AddScoped<IPqrsService, PqrsService>();
builder.Services.AddScoped<IPerformanceSeatService, PerformanceSeatService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
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
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

