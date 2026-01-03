using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UsersService.Infrastructure.DbContext;

namespace UsersService.Infrastructure.Persistence;

/// <summary>
/// Factory for creating UsersServiceDbContext instances at design time (e.g., for migrations).
/// This class implements IDesignTimeDbContextFactory to provide a way to create
/// the DbContext with the appropriate configuration when running design-time tools.
/// </summary>
public class UsersServiceDbContextFactory
    : IDesignTimeDbContextFactory<UsersServiceDbContext>
{
    public UsersServiceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersServiceDbContext>();

        // LOCAL DEV connection (used ONLY for migrations)
        var connectionString =
            "Host=localhost;Port=5432;Database=Users;Username=postgres;Password=postgres"; // temporarilly harcoded

        optionsBuilder.UseNpgsql(connectionString);

        return new UsersServiceDbContext(optionsBuilder.Options);
    }
}
