using System.Security.Claims;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(ForumDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PostResponse>>> GetPosts()
    {
        return await context.Posts.Where(t => !t.IsDeleted).Select(p => p.Map()).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponse>> GetPost(Guid id)
    {
        var item = await context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        return item.Map();
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<PostResponse>> CreatePost(CreatePostRequest post)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID missing from token.");
            
        var item = new Post { Title = post.Title, Content = post.Content, AuthorId = Guid.Parse(userId) };
        var entity = context.Posts.Add(item);
        await context.SaveChangesAsync();

        return entity.Entity.Map();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<PostResponse>> UpdatePost(Guid id, UpdatePostRequest post)
    {
        var item = await context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        item.Title = post.Title;
        item.Content = post.Content;
        await context.SaveChangesAsync();

        return item.Map();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        var item = await context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        item.IsDeleted = true;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
