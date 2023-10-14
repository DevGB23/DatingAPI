using Dating_WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<AppUser> Users { get; set; } = null!;

}
