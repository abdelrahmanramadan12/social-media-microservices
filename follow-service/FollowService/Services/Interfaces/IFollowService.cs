namespace Services.Interfaces
{
    public interface IFollowService
    {
        void Follow(string userId, string otherId);

        void Unfollow(string userId, string otherId);

        bool IsFollowing(string userId, string otherId);

        bool IsFollower(string userId, string otherId);

        ICollection<String> ListFollowing(string userId);

        ICollection<String> ListFollowers(string userId);
    }
}
