using Service.Events;

namespace Service.Interfaces.PostServices
{
    public interface IPostService
    {
        Task HandlePostEventAsync(PostEvent post);
    }
}
