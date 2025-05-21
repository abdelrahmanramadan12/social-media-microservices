namespace Service.Interfaces
{
    public interface IPostDeletedService
    {
        Task HandlePostDeletedAsync(string postId);
    }
}
