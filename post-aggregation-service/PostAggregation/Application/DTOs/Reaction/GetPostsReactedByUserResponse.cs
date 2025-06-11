namespace Application.DTOs.Reaction
{
    public enum ReactionType
    {
        Like = 0,
        Love = 1,
        Haha = 2,
        Wow = 3,
        Sad = 4,
        Angry = 5
    }
    public class Reaction
    {
        public string PostId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
    public class GetPostsReactedByUserResponse
    {
        public List<Reaction> Reactions { get; set; } = new();
        public string Next { get; set; } = string.Empty;
        public bool HasMore { get; set; }
    }
}
