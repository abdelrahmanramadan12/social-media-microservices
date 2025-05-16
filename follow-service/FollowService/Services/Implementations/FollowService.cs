using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FollowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Follow(string userId, string otherId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            var other = _unitOfWork.Users.Find(u => u.UserId == otherId);

            if (user == null)
            {
                user = new User
                {
                    UserId = userId,
                    Followers = new List<string>(),
                    Following = new List<string>()
                };
                _unitOfWork.Users.Add(user);
            }

            if (other == null)
            {
                other = new User
                {
                    UserId = otherId,
                    Followers = new List<string>(),
                    Following = new List<string>()
                };
                _unitOfWork.Users.Add(other);
            }

            if (!user.Following.Contains(otherId))
            {
                user.Following.Add(otherId);
            }

            if (!other.Followers.Contains(userId))
            {
                other.Followers.Add(userId);
            }

            _unitOfWork.Save();
        }

        public void Unfollow(string userId, string otherId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            var other = _unitOfWork.Users.Find(u => u.UserId == otherId);

            if (user != null && user.Following != null && user.Following.Contains(otherId))
            {
                user.Following.Remove(otherId);
            }

            if (other != null && other.Followers != null && other.Followers.Contains(userId))
            {
                other.Followers.Remove(userId);
            }

            _unitOfWork.Save();
        }

        public bool IsFollowing(string userId, string otherId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            return user?.Following?.Contains(otherId) ?? false;
        }

        public bool IsFollower(string userId, string otherId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            return user?.Followers?.Contains(otherId) ?? false;
        }

        public ICollection<String> ListFollowing(string userId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            return user?.Following?.ToList() ?? [];
        }

        public ICollection<String> ListFollowers(string userId)
        {
            var user = _unitOfWork.Users.Find(u => u.UserId == userId);
            return user?.Followers?.ToList() ?? [];
        }
    }
}
