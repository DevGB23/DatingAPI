namespace Dating_WebAPI.Entities;
public class UsersLike
{   
    public AppUser? SourceUser { get; set; }
    public int SourceUserId { get; set; }
    public AppUser? TargetUser { get; set; }
    public int TargetUserId { get; set; }
}
