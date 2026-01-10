using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotificationsService.Infrastructure.Persistence;

/// <summary>
/// Factory for creating UsersServiceDbContext instances at design time (e.g., for migrations).
/// This class implements IDesignTimeDbContextFactory to provide a way to create
/// the DbContext with the appropriate configuration when running design-time tools.
/// </summary>
public class NotificationServiceDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();
        // LOCAL DEV connection (used ONLY for migrations)
        var connectionString =
            "Server=localhost;Port=3306;Database=NotificationsDb;User=root;Password=password;"; // temporarilly harcoded

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
