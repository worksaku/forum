namespace Forum.Models;

public class Post : BaseModel
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public record PostResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedDateTimeUtc,
    DateTime ModifiedDateTimeUtc
);

public record CreatePostRequest(string Title, string Content);

public record UpdatePostRequest(string Title, string Content);

public static class PostMapper
{
    public static PostResponse Map(this Post post) =>
        new(post.Id, post.Title, post.Content, post.CreatedDateTimeUtc, post.ModifiedDateTimeUtc);
}
