using Domain.IRepository;
using Service.Interfaces.PostServices;

namespace Service.Implementations.PostServices
{
    public class PostDeletedService : IPostDeletedService
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IPostRepository _postRepo;

        public PostDeletedService(ICommentRepository commentRepo, IPostRepository postRepo)
        {
            _commentRepo = commentRepo;
            _postRepo = postRepo;
        }

        public async Task HandlePostDeletedAsync(string postId)
        {
            // Check if the post exists
            var post = await _postRepo.GetPostByIdAsync(postId);
            // Delete the post
             await _postRepo.DeletePostAsync(postId);
            // Delete all comments related to this post
            await _commentRepo.DeleteByPostIdAsync(postId);

        }
    }
}
