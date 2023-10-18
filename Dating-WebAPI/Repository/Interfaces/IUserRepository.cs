using System.Linq.Expressions;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;

namespace Dating_WebAPI.Repository.Interfaces;

public interface IUserRepository : IRepository<AppUser>
{
    Task<IEnumerable<MembersDTO>> GetMembersAsync();
    Task<MembersDTO> GetMemberByNameAsync(string username);
    Task<MembersDTO> GetMemberByIdAsync(int id);
    Task UpdateAsync(AppUser entity);
    Task RemoveAsync(AppUser entity);        
}
