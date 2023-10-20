using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;

namespace Dating_WebAPI.Repository.Interfaces;
public interface IUserLikeRepository
{
    Task<UsersLike> GetUserLike (int userSourceId, int userTargetId); 
    Task<AppUser> GetUserWithLikes (int userId); 
    Task<IEnumerable<LikeDTO>> GetUserLikes (string predicate, int userId); 
}
