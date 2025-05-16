using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Models;
using Zion.Reminder.Data;
using Zion.Reminder.Middleware;
using Zion.Reminder.Services;
using Zion.Reminder.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

SetupConfiguration(builder);
SetupServices(builder);


// Ensure appsettings.OpenAI.json is loaded
builder.Configuration.AddJsonFile("appsettings.OpenAI.json", optional: true, reloadOnChange: true);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zion Reminder API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add global error handling middleware
app.UseErrorHandler();

// Add a simple hello world endpoint for testing
app.MapGet("/hello", () => "Hello World from Zion Reminder API!");

app.MapControllers();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Apply migrations if they're pending
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();


void SetupConfiguration(WebApplicationBuilder builder)
{
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
}

void SetupServices(WebApplicationBuilder builder)
{
    // JWT Authentication setup using IOptions<JwtSettings>
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
        throw new InvalidOperationException("JWT secret is not configured. Please set Jwt__Secret in appsettings or environment variables.");
    var jwtSecret = jwtSettings.Secret;
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();

    // Register custom services
    builder.Services.AddScoped<Zion.Reminder.Services.IEventProcessor, Zion.Reminder.Services.EventProcessor>();

    // Configure PostgreSQL database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    // Add Swagger/OpenAPI support
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
        });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Register notification services
    builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();
    builder.Services.AddScoped<INotificationProcessorResolver, NotificationProcessorResolver>();
    builder.Services.AddHostedService<NotificationWorker>();

    // Register channel processors
    builder.Services.AddScoped<IChannelProcessor, EmailChannelProcessor>();
    builder.Services.AddScoped<IChannelProcessor, TeamsChannelProcessor>();

    builder.Services.AddScoped<IMessageGenerator, MessageGenerator>();

    // Register OpenAISettings from configuration
    builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));

    // Register IOpenAIService using options
    builder.Services.AddScoped<IOpenAIService>(sp =>
    {
        var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenAISettings>>().Value;
        return new OpenAIService(new OpenAI.GPT3.OpenAiOptions { ApiKey = options.ApiKey });
    });
}
