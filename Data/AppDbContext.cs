using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Models;

namespace Zion.Reminder.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Reminder> Reminders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed some initial data
        modelBuilder.Entity<Reminder>().HasData(
            new Reminder
            {
                Id = 1,
                Title = "Complete project documentation",
                Description = "Finish the API documentation for the reminder service",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = Priority.High
            },
            new Reminder
            {
                Id = 2,
                Title = "Weekly team meeting",
                Description = "Discuss project progress and roadblocks",
                DueDate = DateTime.UtcNow.AddDays(3),
                Priority = Priority.Medium
            }
        );
    }
}
