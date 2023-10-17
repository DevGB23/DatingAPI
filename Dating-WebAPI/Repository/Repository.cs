using System.Linq.Expressions;
using Dating_WebAPI.Data;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Dating_WebAPI.Repository;

public class Repository<T> : IRepository<T> where T : class
{

    private readonly DataContext _db;
    internal DbSet<T> dbset;

    public Repository(DataContext db)
    {
        _db = db;
        this.dbset = _db.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null, Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = dbset;

        if ( filter is not null)
        {
            query = query.Where(filter);
        };

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        

        return await query.ToListAsync();
    }

    public async Task<T> GetAsync(string? includeProperties = null, bool tracked = false, Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = dbset;

        if (tracked)
            {
                query = dbset;
            }
            else
            {
                query = dbset.AsNoTracking();
            }

        
        if ( filter is not null)
        {
            query = query.Where(filter);
        };

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task SaveAsync()
    {
       await _db.SaveChangesAsync();
    }
}

