using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using shared.Model;
using API.Data;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PostContext>(options =>
    options.UseSqlite("Data Source=kreddit.db")); 


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Konfigurer HTTP-pipelinen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Opret databasen ved opstart
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostContext>();
    db.Database.EnsureCreated(); // Opretter databasen og tabellerne
}

// Endpoints for API'en
// Hent alle posts
app.MapGet("/posts", async (PostContext db) =>
{
    var posts = await db.Posts.ToArrayAsync();
    return Results.Ok(posts);
});

// Hent et specifikt post
app.MapGet("/posts/{id}", async (PostContext db, int id) =>
{
    var post = await db.Posts.FindAsync(id);
    return post != null ? Results.Ok(post) : Results.NotFound();
});

// Opret en kommentar til et post
app.MapPost("/posts/{postId}/comments", async (PostContext db, int postId, HttpRequest request) =>
{
    var json = await new StreamReader(request.Body).ReadToEndAsync();
    var data = System.Text.Json.JsonSerializer.Deserialize<CommentRequest>(json);
    if (data == null || string.IsNullOrEmpty(data.Content)) return Results.BadRequest();

    var post = await db.Posts.FindAsync(postId);
    if (post == null) return Results.NotFound();

    var comment = new Comment
    {
        Content = data.Content,
        PostId = postId,
        User = data.UserId
    };
    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Created($"/posts/{postId}/comments/{comment.Id}", comment);
});

// Upvote et post
app.MapPut("/posts/{id}/upvote", async (PostContext db, int id) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();
    post.Votes++;
    await db.SaveChangesAsync();
    return Results.Ok(post);
});

// Kør appen
app.Run();