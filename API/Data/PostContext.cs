using Microsoft.EntityFrameworkCore;
using shared.Model;

namespace API.Data
{
    public class PostContext : DbContext
    {
        public DbSet<User> Users { get; set; } 
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; } 
        public string DbPath { get; }

        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
            DbPath = "bin/Post.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Comment>().ToTable("Comments");
        }
    }
}
