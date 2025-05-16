using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Models;
using Zion.Reminder.Data;
using Zion.Reminder.Middleware;
using Zion.Reminder.Services;
using Zion.Reminder.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

// JWT Authentication setup
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-strong-secret";

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
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() 
    ?? new DatabaseSettings { ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "" };

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(databaseSettings.ConnectionString));

// Configure Email Settings
// First bind from configuration file
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Then override with environment variables if they exist
var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
if (!string.IsNullOrEmpty(emailPassword))
{
    // Override the password from environment variable
    emailSettings.Password = emailPassword;
    builder.Services.PostConfigure<EmailSettings>(options =>
    {
        options.Password = emailPassword;
    });
}

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
