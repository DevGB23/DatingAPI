using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Repository;

public class UserLikeRepository : IUserLikeRepository
{
    private readonly DataContext _context;

    public UserLikeRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UsersLike> GetUserLike(int userSourceId, int userTargetId)
    {
        return await _context.UserLikes.FindAsync(userSourceId, userTargetId);
    }

    public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
    {
        IQueryable<AppUser> users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

        IQueryable<UsersLike> likes = _context.UserLikes.AsQueryable();

        if ( likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        if ( likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        IQueryable<LikeDTO> likeUsers = users.Select(user => new LikeDTO 
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).ImageUrl,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDTO>.CreateAsync(likeUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}