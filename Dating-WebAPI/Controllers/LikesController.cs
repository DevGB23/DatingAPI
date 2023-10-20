using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;
public class LikesController: BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IUserLikeRepository _likesRepository;

    public LikesController(IUserRepository userRepository, IUserLikeRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetAsync(includeProperties: "", tracked: true, u => u.Username == username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.Username == username) return BadRequest("You cannot like yourself");

            UsersLike userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UsersLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            await _userRepository.SaveAsync();

            // return BadRequest("Failed to like user");
            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);
        }
}
