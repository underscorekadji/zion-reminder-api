using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Models;

namespace Zion.Reminder.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!; protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Event entity
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.From).IsRequired().HasMaxLength(255);
            entity.Property(e => e.To).IsRequired().HasMaxLength(255);
            entity.Property(e => e.For).HasMaxLength(255);
            entity.Property(e => e.FromName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ToName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ForName).HasMaxLength(255);

            // One-to-Many relationship with Notification
            entity.HasMany(e => e.Notifications)
                  .WithOne(n => n.Event)
                  .HasForeignKey(n => n.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Notification entity
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChannelAddress).IsRequired().HasMaxLength(255);

            // Many-to-One relationship with Event
            entity.HasOne(n => n.Event)
                  .WithMany(e => e.Notifications).HasForeignKey(n => n.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
