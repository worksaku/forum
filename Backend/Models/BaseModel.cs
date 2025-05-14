namespace Forum.Models;

public abstract class BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedDateTimeUtc { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDateTimeUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
