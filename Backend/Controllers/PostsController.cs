using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(ForumDbContext context) : ControllerBase
{
    private readonly ForumDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<List<PostResponse>>> GetPosts()
    {
        return await _context.Posts.Where(t => !t.IsDeleted).Select(p => p.Map()).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponse>> GetPost(Guid id)
    {
        var item = await _context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        return item.Map();
    }

    [HttpPut]
    public async Task<ActionResult<PostResponse>> CreatePost(CreatePostRequest post)
    {
        var item = new Post { Title = post.Title, Content = post.Content };
        var entity = _context.Posts.Add(item);
        await _context.SaveChangesAsync();

        return entity.Entity.Map();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<PostResponse>> UpdatePost(Guid id, UpdatePostRequest post)
    {
        var item = await _context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        item.Title = post.Title;
        item.Content = post.Content;
        await _context.SaveChangesAsync();

        return item.Map();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        var item = await _context.Posts.FindAsync(id);
        if (item == null || item.IsDeleted)
            return NotFound();

        item.IsDeleted = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
