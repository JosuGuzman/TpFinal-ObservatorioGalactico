using WatchTower.API.Models;
using WatchTower.API.Controllers;
using WatchTower.API.Middlewares;
using WatchTower.API.Services;
using WatchTower.API.Repositories;
using WatchTower.API.BackgroundServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// CORS para frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoint básico de salud
app.MapGet("/api/health", () => Results.Ok(new { status = "API is running", timestamp = DateTime.UtcNow }));

app.Run();

public partial class Program { }
