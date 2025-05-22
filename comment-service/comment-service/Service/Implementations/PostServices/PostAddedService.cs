using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.PostServices;

namespace Service.Implementations.PostServices
{
    public class PostAddedService: IPostAddedService
    {
        private readonly IPostRepository _postRepo;
        public PostAddedService(IPostRepository postRepo)
        {
            _postRepo = postRepo;
        }
        public async Task HandlePostAddedAsync(PostAddedEvent post)
        {

            // Check if the post already exists
            var existingPost = _postRepo.GetPostByIdAsync(post.PostId);
            if (existingPost != null)
            {
                return ;
            }
            // Add the new post
            var newPost = new Domain.Entities.Post
            {
                Id = post.PostId,
                AuthorId = post.PostAuthorId,
                Privacy = post.Privacy,

            };
            var isAdded = await _postRepo.AddPostAsync(newPost);
            if (!isAdded)
            {
                throw new Exception($"Failed to add post with ID {post.PostId}.");
            }
        }
    }
}
