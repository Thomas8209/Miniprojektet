namespace shared.Model;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int Score { get; set; }
    public User User { get; set; }
    public Comment(string content = "", int score = 0, User user = null)
    {
        Content = content;
        Score = score;
        User = user ?? new User();
    }
    public Comment() {
        Id = 0;
        Content = "";
        Score = 0;
    }
}