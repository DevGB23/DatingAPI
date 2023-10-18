namespace Dating_WebAPI.DTOs;
public class UserDTO
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}
