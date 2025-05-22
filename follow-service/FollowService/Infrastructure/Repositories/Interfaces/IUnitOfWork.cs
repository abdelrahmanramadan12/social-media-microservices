namespace Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IFollowRepository Follows { get; }
    }
}