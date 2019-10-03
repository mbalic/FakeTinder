namespace FakeTinder.API.Models
{
    public class Like
    {
        // User that is liking another user
        public int LikerId { get; set; }
        // User that is liked another user
        public int LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}