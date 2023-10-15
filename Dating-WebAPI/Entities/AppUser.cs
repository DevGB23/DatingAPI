using System.ComponentModel.DataAnnotations;

namespace Dating_WebAPI.Entities;
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
}
