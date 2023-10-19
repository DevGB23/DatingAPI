using System.Linq.Expressions;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Helpers;

namespace Dating_WebAPI.Repository.Interfaces;

public interface IUserRepository : IRepository<AppUser>
{
    Task<PagedList<MembersDTO>> GetMembersAsync(UserParams userParams);
    Task<MembersDTO> GetMemberByNameAsync(string username);
    Task<MembersDTO> GetMemberByIdAsync(int id);
    Task UpdateAsync(AppUser entity);
    Task RemoveAsync(AppUser entity);        
}
