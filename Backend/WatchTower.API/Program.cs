using WatchTower.API.Data;
using WatchTower.API.Repositories;
using WatchTower.API.Services;
using WatchTower.API.BackgroundServices;
using WatchTower.API.Endpoints;
using WatchTower.API.Middleware;
using WatchTower.API.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

// Database
builder.Services.AddScoped<IDbConnectionFactory>(_ => 
    new MySqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICelestialBodyRepository, CelestialBodyRepository>();
builder.Services.AddScoped<IDiscoveryRepository, DiscoveryRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Background Services
builder.Services.AddHostedService<NASASyncService>();

// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig?.Issuer,
            ValidAudience = jwtConfig?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig?.Key ?? ""))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapAuthEndpoints();
app.MapCatalogEndpoints();
app.MapDiscoveryEndpoints();

// Health check
app.MapGet("/", () => "WatchTower API is running!");
app.MapGet("/api/health", () => Results.Ok(new { 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

app.Run();

public partial class Program { }