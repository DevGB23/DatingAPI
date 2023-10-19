
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Repository;

public class UserRepository : Repository<AppUser>, IUserRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public UserRepository(DataContext db, IMapper mapper) : base(db)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<MembersDTO> GetMemberByNameAsync(string username)
    {
        MembersDTO memberOut = new();

        MembersDTO? member = await _db.Users.Where(u => u.Username == username)
                        .ProjectTo<MembersDTO>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();
        
        if (member is not null) memberOut = member;

        return memberOut;
    }
    public async Task<MembersDTO> GetMemberByIdAsync(int id)
    {
        MembersDTO memberOut = new();

        MembersDTO? member = await _db.Users.Where(u => u.Id == id)
                        .ProjectTo<MembersDTO>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();
        
        if (member is not null) memberOut = member;

        return memberOut;
    }
    public async Task<PagedList<MembersDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = _db.Users.AsQueryable();

        query = query.Where(u => u.Username != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        return await PagedList<MembersDTO>.CreateAsync (
            query.AsNoTracking().ProjectTo<MembersDTO>(_mapper.ConfigurationProvider),
            userParams.PageNumber, 
            userParams.PageSize
        );
    }

    public async Task UpdateAsync(AppUser entity)
    {
        _db.Users.Update(entity);
        await _db.SaveChangesAsync();        
    }

    public async Task RemoveAsync(AppUser entity)
    {
        _db.Users.Remove(entity);
        await _db.SaveChangesAsync();        
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }
}
