var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configurar servicios
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
    options.Filters.Add<ApiKeyAuthFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            var origins = builder.Configuration.GetSection("AppSettings:ApiSettings:CorsOrigins").Get<string[]>();
            if (origins != null && origins.Length > 0)
            {
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
        });
});

// Configurar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("AppSettings:Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AstronomerOrAdmin", policy => policy.RequireRole("Astronomer", "Admin"));
    options.AddPolicy("ResearcherOrAbove", policy => policy.RequireRole("Researcher", "Astronomer", "Admin"));
});

// Configurar Swagger
if (builder.Configuration.GetValue<bool>("AppSettings:ApiSettings:EnableSwagger"))
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Observatorio WatchTower API",
            Version = "v1",
            Description = "API REST para el Observatorio Astronómico WatchTower",
            Contact = new OpenApiContact
            {
                Name = "Equipo de Desarrollo",
                Email = "dev@observatorio.watchtower"
            }
        });

        // Configurar seguridad JWT en Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Configurar seguridad API Key en Swagger
        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "API Key authorization",
            Name = "X-API-Key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// Configurar capa de infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar servicios personalizados
builder.Services.AddScoped<RoleAuthorizationFilter>();

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    if (builder.Configuration.GetValue<bool>("AppSettings:ApiSettings:EnableSwagger"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Observatorio API v1");
            c.RoutePrefix = "api-docs";
        });
    }
}

// Middleware personalizados
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ruta por defecto
app.MapGet("/", () => 
{
    return Results.Ok(new 
    { 
        message = "Bienvenido a la API del Observatorio WatchTower",
        version = "1.0.0",
        documentation = "/api-docs",
        endpoints = new 
        {
            auth = "/api/v1/auth",
            galaxies = "/api/v1/galaxies",
            stars = "/api/v1/stars",
            planets = "/api/v1/planets",
            discoveries = "/api/v1/discoveries",
            articles = "/api/v1/articles",
            events = "/api/v1/events",
            users = "/api/v1/users",
            dashboard = "/api/v1/dashboard",
            profile = "/api/v1/profile",
            export = "/api/v1/export"
        }
    });
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

try
{
    Log.Information("Iniciando Observatorio WatchTower API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error al iniciar la aplicación");
}
finally
{
    Log.CloseAndFlush();
}