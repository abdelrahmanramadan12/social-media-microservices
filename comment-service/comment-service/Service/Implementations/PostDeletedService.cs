using Domain.IRepository;
using Service.Interfaces;

namespace Service.Implementations
{
    public class PostDeletedService : IPostDeletedService
    {
        private readonly ICommentRepository _commentRepo;

        public PostDeletedService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public async Task HandlePostDeletedAsync(string postId)
            => await _commentRepo.DeleteByPostIdAsync(postId);

    }
}
