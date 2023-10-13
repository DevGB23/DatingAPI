using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

}
