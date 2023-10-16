using System.ComponentModel.DataAnnotations.Schema;

namespace Dating_WebAPI.Entities;
[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public string PublicId { get; set; } = string.Empty;
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = new();
}