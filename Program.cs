using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Seeders;
using service_eventos_eventual.Services.BackgroundServices;
using service_eventos_eventual.Services.Implementations;
using service_eventos_eventual.Services.Interfaces;
using Stripe;

//Pruebas
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

// ─── JWT ───────────────────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer            = true,
            ValidateAudience          = false,
            ValidateLifetime          = true,
            ValidateIssuerSigningKey  = true,
            ValidIssuer               = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey          = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            // Laravel emite "sub" como número entero — .NET lo lee como string, OK
            NameClaimType  = "sub",
            RoleClaimType  = "role"
        };
    });

builder.Services.AddAuthorization();

// ─── Controllers y OpenAPI ─────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// ─── Database ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<TeatroEventsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
});

// ─── Services ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IPlayService,        PlayService>();
builder.Services.AddScoped<ISeatService,        SeatService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IFavoriteService,    FavoriteService>();
builder.Services.AddScoped<IPurchaseService,    PurchaseService>();
builder.Services.AddScoped<ITicketService,      TicketService>();
builder.Services.AddScoped<IPqrsService,        PqrsService>();
builder.Services.AddScoped<IQrService,          QrService>();
builder.Services.AddScoped<IMetricsService,     MetricsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddHostedService<PerformanceStatusUpdaterService>();

// ─── CORS ─────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// ─── Seed ──────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TeatroEventsDbContext>();
    await SeatSeeder.SeedAsync(context);
}

// ─── Middleware pipeline (el orden importa) ────────────────────────────────
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowAll");            // CORS va primero
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
