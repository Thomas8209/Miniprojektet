namespace shared.Model;

public class Post {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Score { get; set; }
    public User User { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public Post(User user, string title = "", string content = "", int score = 0) {
        Title = title;
        Content = content;
        Score = score;
        User = user;
    }
    public Post() {
        Id = 0;
        Title = "";
        Content = "";
        Score = 0;
        User = new User(); 
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Content: {Content}, Score: {Score}, User: {User}";
    }
}