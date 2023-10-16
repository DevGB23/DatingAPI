using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;

namespace Dating_WebAPI.DTOs;
public class MembersDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int Age { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public List<PhotoDTO>? Photos { get; set; }

    // public int GetAge()
    // {
    //     return DateOfBirth.CalculateAge();
    // }

}