using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Helpers;

namespace Dating_WebAPI.Repository.Interfaces;
public interface IUserLikeRepository
{
    Task<UsersLike> GetUserLike (int userSourceId, int userTargetId); 
    Task<AppUser> GetUserWithLikes (int userId); 
    Task<PagedList<LikeDTO>> GetUserLikes (LikesParams likesParams); 
}
