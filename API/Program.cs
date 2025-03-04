using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using shared.Model; 


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
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
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Opretter databasen og tabellerne
}

// Endpoints for API'en
// Hent alle posts
app.MapGet("/posts", async (AppDbContext db) =>
{
    var posts = await db.Posts.ToArrayAsync();
    return Results.Ok(posts);
});

// Hent et specifikt post
app.MapGet("/posts/{id}", async (AppDbContext db, int id) =>
{
    var post = await db.Posts.FindAsync(id);
    return post != null ? Results.Ok(post) : Results.NotFound();
});

// Opret en kommentar til et post
app.MapPost("/posts/{postId}/comments", async (AppDbContext db, int postId, HttpRequest request) =>
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
        UserId = data.UserId
    };
    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Created($"/posts/{postId}/comments/{comment.Id}", comment);
});

// Upvote et post
app.MapPut("/posts/{id}/upvote", async (AppDbContext db, int id) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();
    post.Votes++;
    await db.SaveChangesAsync();
    return Results.Ok(post);
});

// Kør appen
app.Run();

// Nødvendige klasser
public class AppDbContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

public record CommentRequest(string Content, int UserId);