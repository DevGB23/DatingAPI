using System.Security.Claims;
using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
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

    public UsersController(IUserRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
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
        var username =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username is null) return NotFound("Username not found");

        AppUser? user = await _repo.GetAsync(includeProperties: "Photos", tracked: false, u => u.Username == username);
        
        _mapper.Map(memberUpdateDTO, user);

        if (user is null) return NotFound("User not found");

        await _repo.UpdateAsync(user);

        return Ok();
    }
}