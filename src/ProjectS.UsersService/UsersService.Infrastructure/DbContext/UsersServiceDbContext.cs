using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities;
using UsersService.Infrastructure.Outbox;

namespace UsersService.Infrastructure.DbContext;

public class UsersServiceDbContext(DbContextOptions<UsersServiceDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);

            b.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value)
                 .HasColumnName("Email")
                 .IsRequired();
            });
        });

        builder.Entity<OutboxMessage>(b =>
       {
           b.HasKey(x => x.Id);
           b.Property(x => x.Type).IsRequired();
           b.Property(x => x.Content).IsRequired();
           b.Property(x => x.OccurredOnUtc).IsRequired();
       });
    }
}
