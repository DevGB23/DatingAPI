using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _photoService = photoService;
    }

    
    [HttpGet]
    public async Task<ActionResult<PagedList<MembersDTO>>> GetAllUsersAsync([FromQuery]UserParams userParams)
    {

        string gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());

        userParams.CurrentUsername = User.GetUsername();

        if (string.IsNullOrEmpty(userParams.Gender)) {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        PagedList<MembersDTO> userList = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(
            new PaginationHeader(userList.CurrentPage, userList.PageSize, userList.TotalCount, userList.TotalPages));

        return Ok(userList);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MembersDTO>> GetUserByIdAsync(int id)
    {
        MembersDTO? member = await _unitOfWork.UserRepository.GetMemberByIdAsync(id);
            
        if ( member is null ) return NotFound($"User with ID {id} was not found");

        return Ok(member);
    }

    
    [HttpGet("{username}")]
    public async Task<ActionResult<MembersDTO>> GetUserByUsernameAsync(string username)
    {
        MembersDTO? member = await _unitOfWork.UserRepository.GetMemberByNameAsync(username);
            
        if ( member is null ) return NotFound($"User with username {username} was not found");
        
        return Ok(member);
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        AppUser? user = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "Photos", tracked: false, u => u.UserName == User.GetUsername());
        
        _mapper.Map(memberUpdateDTO, user);

        if (user is null) return NotFound("User not found");

        await _unitOfWork.UserRepository.UpdateAsync(user);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to Update user");
    }


    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto (IFormFile file)
    {
        AppUser? user = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "Photos", tracked: false, u => u.UserName == User.GetUsername());

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

        await _unitOfWork.UserRepository.UpdateAsync(user);
        
        return _mapper.Map<PhotoDTO>(photo);
    }


    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        AppUser? user = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "Photos", tracked: false, u => u.UserName == User.GetUsername());

        if (user is null) return NotFound();

        Photo? photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo"); 

        Photo? currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

        if (currentMain is not null) currentMain.IsMain = false;

        photo.IsMain = true;

        await _unitOfWork.UserRepository.UpdateAsync(user);

        return NoContent();
    }


    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        AppUser? user = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "Photos", tracked: true, u => u.UserName == User.GetUsername());

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

        await _unitOfWork.UserRepository.UpdateAsync(user);

        return Ok();
    }

}