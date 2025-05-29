namespace Application.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IFollowRepository Follows { get; }
    }
}