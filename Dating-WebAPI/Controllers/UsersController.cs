using System.Security.Claims;
using AutoMapper;
using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;

[Authorize]
public class UsersController : BaseApiController
{

    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    private readonly DataContext _dataContext;

    public UsersController(IUserRepository repo, IMapper mapper, IPhotoService photoService, DataContext dataContext)
    {
        _repo = repo;
        _mapper = mapper;
        _photoService = photoService;
        _dataContext = dataContext;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembersDTO>>> GetAllUsersAsync()
    {
        IEnumerable<MembersDTO> userList = await _repo.GetMembersAsync();

        return Ok(userList);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MembersDTO>> GetUserByIdAsync(int id)
    {
        MembersDTO? member = await _repo.GetMemberByIdAsync(id);
            
        if ( member is null ) return NotFound($"User with ID {id} was not found");

        return Ok(member);
    }

    
    [HttpGet("{username}")]
    public async Task<ActionResult<MembersDTO>> GetUserByUsernameAsync(string username)
    {
        MembersDTO? member = await _repo.GetMemberByNameAsync(username);
            
        if ( member is null ) return NotFound($"User with username {username} was not found");
        
        return Ok(member);
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        AppUser? user = await _repo.GetAsync(includeProperties: "Photos", tracked: false, u => u.Username == User.GetUsername());
        
        _mapper.Map(memberUpdateDTO, user);

        if (user is null) return NotFound("User not found");

        await _repo.UpdateAsync(user);

        return Ok();
    }


    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto (IFormFile file)
    {
        AppUser? user = await _repo.GetAsync(includeProperties: "Photos", tracked: false, u => u.Username == User.GetUsername());

        if (user is null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error is not null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            ImageUrl = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            AppUserId = user.Id,
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        await _repo.UpdateAsync(user);
        
        return _mapper.Map<PhotoDTO>(photo);
    }


    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        AppUser? user = await _repo.GetAsync(includeProperties: "Photos", tracked: false, u => u.Username == User.GetUsername());

        if (user is null) return NotFound();

        Photo? photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo"); 

        Photo? currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

        if (currentMain is not null) currentMain.IsMain = false;

        photo.IsMain = true;

        await _repo.UpdateAsync(user);

        return NoContent();
    }


    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        AppUser? user = await _repo.GetAsync(includeProperties: "Photos", tracked: true, u => u.Username == User.GetUsername());

        if (user is null) return NotFound();

        Photo? photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("You Cannot delete your main photo");

        if (photo.PublicId is not null) 
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error is not null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        await _repo.UpdateAsync(user);

        return Ok();
    }

}