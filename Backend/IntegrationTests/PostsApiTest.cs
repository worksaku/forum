using Forum.IntegrationTests.Utils;
using Forum.Models;
using Xunit;

namespace Forum.IntegrationTests;

public class PostApiTests(CustomWebAppFactory factory) : TestBase(factory), IClassFixture<CustomWebAppFactory>
{

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
        var user = await TestUtils.CreateTestUser(db: Db);
        var post = await TestUtils.CreateTestPost(user.Id, db: Db);

        // Act
        var posts = await Client.GetFromJsonAsync<List<PostResponse>>("/api/posts");

        // Assert
        Assert.NotNull(posts);
        Assert.Single(posts);
        Assert.Equal(post.Title, posts[0].Title);
        Assert.Equal(post.Content, posts[0].Content);
    }

    [Fact]
    public async Task GetPosts_DoesntReturn_SoftDeleted()
    {
        // Arrange
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        Db.Posts.Add(await TestUtils.CreateTestPost(user.Entity.Id, isDeleted: true));
        await Db.SaveChangesAsync();

        // Act
        var posts = await Client.GetFromJsonAsync<List<PostResponse>>("/api/posts");

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
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        var post = Db.Posts.Add(await TestUtils.CreateTestPost(user.Entity.Id));
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.GetFromJsonAsync<PostResponse>($"/api/posts/{post.Entity.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(post.Entity.Title, result.Title);
        Assert.Equal(post.Entity.Content, result.Content);
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
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        var post = Db.Posts.Add(await TestUtils.CreateTestPost(user.Entity.Id, isDeleted: true));
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.GetAsync($"/api/posts/{post.Entity.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
    #region CreatePost

    [Fact]
    public async Task CreatePost_ReturnsUnauthorized()
    {
        // Arrange
        var post = new CreatePostRequest("Hello test!", "This is a test post.");

        // Act
        var result = await Client.PutAsJsonAsync("/api/posts", post);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ReturnsPost()
    {
        // Arrange
        var testUser = await TestUtils.CreateTestUser();
        Db.Users.Add(testUser);
        await Db.SaveChangesAsync();
        TestUtils.LoginTestUser(Client, Factory, testUser);

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
    public async Task UpdatePost_ReturnsUnauthorized()
    {
        // Arrange
        var post = new UpdatePostRequest("Updated title", "Updated content");
        // Act
        var result = await Client.PatchAsJsonAsync("/api/posts/00000000-0000-0000-0000-000000000000", post);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_ReturnsForbidden()
    {
        // Arrange
        var user1 = await TestUtils.CreateTestUser(db: Db);
        var user2 = await TestUtils.CreateTestUser("anotheruser", "another@example.com", db: Db);
        var post = await TestUtils.CreateTestPost(user1.Id, db: Db);
        await Db.SaveChangesAsync();

        TestUtils.LoginTestUser(Client, Factory, user2);

        var updatePost = new UpdatePostRequest("Updated title", "Updated content");

        // Act
        var result = await Client.PatchAsJsonAsync($"/api/posts/{post.Id}", updatePost);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_ReturnsPost()
    {
        // Arrange
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        var post = Db.Posts.Add(await TestUtils.CreateTestPost(user.Entity.Id));
        await Db.SaveChangesAsync();

        TestUtils.LoginTestUser(Client, Factory, user.Entity);

        var updatePost = new UpdatePostRequest("Updated title", "Updated content");

        // Act
        var result = await Client.PatchAsJsonAsync($"/api/posts/{post.Entity.Id}", updatePost);

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
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        await Db.SaveChangesAsync();
        TestUtils.LoginTestUser(Client, Factory, user.Entity);
        var updatePost = new UpdatePostRequest("Updated title", "Updated content");

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
        var user = Db.Users.Add(await TestUtils.CreateTestUser());
        var post = Db.Posts.Add(await TestUtils.CreateTestPost(user.Entity.Id, isDeleted: true));
        await Db.SaveChangesAsync();
        TestUtils.LoginTestUser(Client, Factory, user.Entity);

        var updatePost = new UpdatePostRequest("Updated title", "Updated content");

        // Act
        var result = await Client.PatchAsJsonAsync($"/api/posts/{post.Entity.Id}", updatePost);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
    #region DeletePost

    [Fact]
    public async Task DeletePost_ReturnsNoContent()
    {
        // Arrange
        var user = await TestUtils.CreateTestUser(db: Db);
        var post = await TestUtils.CreateTestPost(user.Id, db: Db);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.DeleteAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task DeletePost_ReturnsForbidden()
    {
        // Arrange
        var user1 = await TestUtils.CreateTestUser(db: Db);
        var user2 = await TestUtils.CreateTestUser("anotheruser", "another@example.com", db: Db);
        var post = await TestUtils.CreateTestPost(user1.Id, db: Db);
        await Db.SaveChangesAsync();

        TestUtils.LoginTestUser(Client, Factory, user2);

        // Act
        var result = await Client.DeleteAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, result.StatusCode);
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
        var user = await TestUtils.CreateTestUser(db: Db);
        var post = await TestUtils.CreateTestPost(user.Id, isDeleted: true, db: Db);
        await Db.SaveChangesAsync();

        // Act
        var result = await Client.DeleteAsync($"/api/posts/{post.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
    #endregion
}
