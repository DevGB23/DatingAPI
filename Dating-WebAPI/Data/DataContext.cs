using Dating_WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<UsersLike> UserLikes { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UsersLike>()
            .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        builder.Entity<UsersLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UsersLike>()
        .HasOne(s => s.TargetUser)
        .WithMany(l => l.LikedByUsers)
        .HasForeignKey(s => s.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);
    }

}
