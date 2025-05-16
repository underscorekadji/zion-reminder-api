using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure PostgreSQL database
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() 
    ?? new DatabaseSettings { ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "" };

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(databaseSettings.ConnectionString));

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseAuthorization();

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
