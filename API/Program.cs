using Microsoft.EntityFrameworkCore;
using API.Data;
using shared.Model;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API kan bruges fra andre domæner
var AllowAll = "_AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAll, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// tilføjer DBcontext for databaseforbindelse
builder.Services.AddDbContext<PostContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

//  tilføjer DataRepository, så det kan bruges i endpoints
builder.Services.AddScoped<DataRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataRepository = scope.ServiceProvider.GetRequiredService<DataRepository>();
    dataRepository.SeedData();
}

app.UseHttpsRedirection();
app.UseCors(AllowAll);

// API Endpoints

// Test route
app.MapGet("/", () => new { message = "Hej Verden" });

// Get all posts
app.MapGet("/api/posts", async (DataRepository repo) =>
{
    return await repo.GetPosts();
});

// Get a single post by ID
app.MapGet("/api/posts/{id}", (DataRepository repo, int id) =>
{
    return (repo.GetPost(id) is Post post)
        ? Results.Ok(post)
        : Results.NotFound(new { message = "Post not found" });
});


// Create a new post
app.MapPost("/api/posts", (DataRepository repo, Post post) =>
{
    return repo.CreatePost(post);
});

// Upvote a post
app.MapPost("/api/posts/{id}/upvote", (DataRepository repo, int id) =>
{
    return repo.UpvotePost(id);
});

// Downvote a post
app.MapPost("/api/posts/{id}/downvote", (DataRepository repo, int id) =>
{
    return repo.DownvotePost(id);
});

// Add a comment to a post
app.MapPost("/api/posts/{id}/comments", (DataRepository repo, int id, Comment comment) =>
{
    return repo.CreateComment(id, comment);
});

// Upvote a comment
app.MapPost("/api/posts/{postId}/comments/{commentId}/upvote", (DataRepository repo, int postId, int commentId) =>
{
    return repo.UpvoteComment(postId, commentId);
});

// Downvote a comment
app.MapPost("/api/posts/{postId}/comments/{commentId}/downvote", (DataRepository repo, int postId, int commentId) =>
{
    return repo.DownvoteComment(postId, commentId);
});

app.Run();