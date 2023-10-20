using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
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

    public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId)
    {
        IQueryable<AppUser> users = _context.Users.OrderBy(u => u.Username).AsQueryable();

        IQueryable<UsersLike> likes = _context.UserLikes.AsQueryable();

        if ( predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == userId);
            users = likes.Select(like => like.TargetUser);
        }

        if ( predicate == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == userId);
            users = likes.Select(like => like.SourceUser);
        }



        return await users.Select(user => new LikeDTO {
            Username = user.Username,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).ImageUrl,
            City = user.City,
            Id = user.Id
        }).ToListAsync();
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}