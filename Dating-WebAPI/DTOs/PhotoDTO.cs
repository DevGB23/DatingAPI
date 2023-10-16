using Dating_WebAPI.Entities;

namespace Dating_WebAPI.DTOs;
public class PhotoDTO
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsMain { get; set; }

    // public int GetAge()
    // {
    //     return DateOfBirth.CalculateAge();
    // }

}