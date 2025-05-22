namespace Service.Interfaces.PostServices
{
    public interface IPostDeletedService
    {
        Task HandlePostDeletedAsync(string postId);
    }
}
