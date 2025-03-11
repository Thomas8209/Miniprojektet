using Microsoft.EntityFrameworkCore;
using shared.Model;

namespace API.Data
{
    public class DataRepository
    {
        private PostContext db { get; }

        public DataRepository(PostContext db)
        {
            this.db = db;

        }

        //Kommunikation til db herinde:

        public void SeedData()
        {
            Console.WriteLine("🚀 Kører SeedData...");
            // Tjek om der findes en bruger
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Username = "Thomas" });
                db.SaveChanges();
            }

            // Hent brugeren (nu hvor vi er sikre på, at der er en)
            User user = db.Users.FirstOrDefault(u => u.Username == "Thomas")!;

            // Tjek om der findes posts
            if (!db.Posts.Any())
            {
                var post1 = new Post
                {
                    Title = "Hvad er den bedste burger i Danmark?",
                    Content = "Jeg har prøvet mange forskellige burgere, men vil gerne høre jeres bud!",
                    Score = 12,
                    User = user
                };

                var post2 = new Post
                {
                    Title = "Hvordan laver man den perfekte kop kaffe?",
                    Content = "Er det aeropress, filterkaffe eller espresso? Jeg vil gerne høre jeres mening!",
                    Score = 8,
                    User = user
                };

                db.Posts.AddRange(post1, post2);
                db.SaveChanges();

                // 🔥 Tilføj kommentarer direkte til posts
                post1.Comments.Add(new Comment
                {
                    Content = "Burger Shack i Aarhus laver den bedste burger!",
                    Score = 4,
                    User = user
                });

                post2.Comments.Add(new Comment
                {
                    Content = "Aeropress med friskkværnede bønner er vejen frem!",
                    Score = 6,
                    User = user
                });

                db.SaveChanges(); // Gem kommentarer sammen med posts
            }
        }

        public async Task<List<Post>> GetPosts()
        {
            return await db.Posts
                .Include(p => p.User)       
                .Include(p => p.Comments)   
                .ThenInclude(c => c.User)   
                .ToListAsync();
        }

        public Post GetPost(int id)
        {
            return db.Posts
                .Include(p => p.User)       
                .Include(p => p.Comments)   
                .ThenInclude(c => c.User)   
                .FirstOrDefault(p => p.Id == id);
        }


        public Post CreatePost(Post article) {

            if (article == null)
            {
                throw new Exception("Article cannot be null");
            }

            db.Posts.Add(article);
            db.SaveChanges();

            return article;
        }

        public Comment CreateComment(int id, Comment newcomment)
        {
            if (newcomment == null)
            {
                throw new ArgumentNullException(nameof(newcomment), "Comment cannot be null");
            }

            var post = db.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new Exception("Post not found");
            }

            post.Comments.Add(newcomment);
            db.SaveChanges();

            return newcomment;
        }
        public Post UpvotePost(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            post.Score += 1; 
            db.SaveChanges();

            return post;
        }

        public Post DownvotePost(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            post.Score -= 1; 
            db.SaveChanges();

            return post;
        }

        public Comment UpvoteComment(int postId, int commentId)
        {
            var post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            comment.Score += 1; 
            db.SaveChanges();

            return comment;
        }

        public Comment DownvoteComment(int postId, int commentId)
        {
            var post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            comment.Score -= 1; 
            db.SaveChanges();

            return comment;
        }
    }
};
