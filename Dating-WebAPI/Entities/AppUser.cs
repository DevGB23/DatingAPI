using Microsoft.AspNetCore.Identity;

namespace Dating_WebAPI.Entities;
public class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public List<Photo> Photos { get; set; } = new ();

    public List<UsersLike>? LikedByUsers { get; set; }
    public List<UsersLike>? LikedUsers { get; set; }

    public ICollection<Message>? MessagesSent { get; set; }
    public ICollection<Message>? MessagesReceived { get; set; } 

    public ICollection<AppUserRole>? UserRoles { get; set; } 

}

