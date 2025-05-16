using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models;

public class User : BaseModel
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public ICollection<Post> Posts { get; set; } = [];
}
