using System.Net.Http.Json;
using Forum.Models;
using Xunit;

namespace Forum.IntegrationTests;

public class PostApiTests : TestBase, IClassFixture<CustomWebAppFactory>
{
    public PostApiTests(CustomWebAppFactory factory)
        : base(factory) { }

    #region GetPosts

    [Fact]
    public async Task GetPosts_ReturnsEmptyList()
    {
        // Act
        var posts = await Client.GetFromJsonAsync<List<PostResponse>>("/api/posts");

        // Assert
        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetPosts_ReturnsPosts()
    {
        // Arrange
        Db.Posts.Add(new Post { Title = "Hello test!", Content = "This is a test post." });
        await Db.SaveChangesAsync();

        // Act
        var posts = await Client.GetFromJsonAsync<List<Post>>("/api/posts");

        // Assert
        Assert.NotNull(posts);
        Assert.Single(posts);
        Assert.Equal("Hello test!", posts[0].Title);
    }

    [Fact]
    public async Task GetPosts_DoesntReturn_SoftDeleted()
    {
        // Arrange
        Db.Posts.Add(
            new Post
            {
                Title = "Hello test!",
                Content = "This is a test post.",
                IsDeleted = true,
            }
        );
        await Db.SaveChangesAsync();

        // Act
        var posts = await Client.GetFromJsonAsync<List<Post>>("/api/posts");

        // Assert
        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    #endregion
    #region GetPost

    [Fact]
    public async Task GetPost_ReturnsPost()
    {
        // Arrange
        var post = new Post { Title = "Hello test!", Content = "This is a test post." };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.GetFromJsonAsync<PostResponse>($"/api/posts/{post.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(post.Title, result.Title);
    }

    [Fact]
    public async Task GetPost_ReturnsNotFound()
    {
        // Act
        var result = await Client.GetAsync("/api/posts/00000000-0000-0000-0000-000000000000");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetPost_ReturnsNotFound_SoftDeleted()
    {
        // Arrange
        var post = new Post
        {
            Title = "Hello test!",
            Content = "This is a test post.",
            IsDeleted = true,
        };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.GetAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
    #region CreatePost

    [Fact]
    public async Task CreatePost_ReturnsPost()
    {
        // Arrange
        var post = new CreatePostRequest("Hello test!", "This is a test post.");

        // Act
        var result = await Client.PutAsJsonAsync("/api/posts", post);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        var createdPost = await result.Content.ReadFromJsonAsync<PostResponse>();
        Assert.NotNull(createdPost);
        Assert.Equal(post.Title, createdPost.Title);
    }

    #endregion
    #region UpdatePost

    [Fact]
    public async Task UpdatePost_ReturnsPost()
    {
        // Arrange
        var post = new Post { Title = "Hello test!", Content = "This is a test post." };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        var updatePost = new UpdatePostRequest("Updated title", "Updated content");

        // Act
        var result = await Client.PatchAsJsonAsync($"/api/posts/{post.Id}", updatePost);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        var updatedPost = await result.Content.ReadFromJsonAsync<PostResponse>();
        Assert.NotNull(updatedPost);
        Assert.Equal(updatePost.Title, updatedPost.Title);
    }

    [Fact]
    public async Task UpdatePost_ReturnsNotFound()
    {
        // Arrange
        var updatePost = new UpdatePostRequest("Updated title", "Updated content");;

        // Act
        var result = await Client.PatchAsJsonAsync(
            "/api/posts/00000000-0000-0000-0000-000000000000",
            updatePost
        );

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_ReturnsNotFound_SoftDeleted()
    {
        // Arrange
        var post = new Post
        {
            Title = "Hello test!",
            Content = "This is a test post.",
            IsDeleted = true,
        };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        var updatePost = new UpdatePostRequest("Updated title", "Updated content");;

        // Act
        var result = await Client.PatchAsJsonAsync($"/api/posts/{post.Id}", updatePost);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
    #region DeletePost

    [Fact]
    public async Task DeletePost_ReturnsNoContent()
    {
        // Arrange
        var post = new Post { Title = "Hello test!", Content = "This is a test post." };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.DeleteAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task DeletePost_ReturnsNotFound()
    {
        // Act
        var result = await Client.DeleteAsync("/api/posts/00000000-0000-0000-0000-000000000000");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeletePost_ReturnsNotFound_SoftDeleted()
    {
        // Arrange
        var post = new Post
        {
            Title = "Hello test!",
            Content = "This is a test post.",
            IsDeleted = true,
        };
        Db.Posts.Add(post);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.DeleteAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
}
