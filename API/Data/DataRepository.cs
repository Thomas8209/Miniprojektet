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

        public List<Post> GetPosts()
        {

        }
        public Post GetPost(int id)
        {

        }

        public Post AddPost(Post article) { }

        public Comment AddComent(int id)
        {

        }
    }
};
