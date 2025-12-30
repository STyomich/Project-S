using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities;

namespace UsersService.Infrastructure.DbContext;

public class UsersServiceDbContext(DbContextOptions<UsersServiceDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    
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
    }
}
