namespace Dating_WebAPI.Entities;
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
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
}

