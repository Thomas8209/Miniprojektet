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
        }

        public async Task<List<Post>> GetPosts()
        {
            return await db.Posts.ToListAsync();
        }
        public Post GetPost(int id)
        {
            return db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == id);
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
