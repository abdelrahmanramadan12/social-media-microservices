using Application.Abstractions;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users { get; }
        public IFollowRepository Follows { get; }

        public UnitOfWork(IUserRepository userRepository, IFollowRepository followRepository)
        {
            Users = userRepository;
            Follows = followRepository;
        }
    }
}