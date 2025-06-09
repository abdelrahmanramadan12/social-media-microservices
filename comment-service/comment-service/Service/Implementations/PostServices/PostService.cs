using Domain.IRepository;
using Service.Enums;
using Service.Events;
using Service.Interfaces.PostServices;

namespace Service.Implementations.PostServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly ICommentRepository _commentRepo;
        public PostService(IPostRepository postRepo, ICommentRepository commentRepo)
        {
            _postRepo = postRepo;
            _commentRepo = commentRepo;
        }
        public async Task HandlePostEventAsync(PostEvent post)
        {

            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post event cannot be null.");
            }
            if (post.EventType == EventType.Create)
            {
                var existingPost = await _postRepo.GetPostByIdAsync(post.PostId);
                if (existingPost != null)
                {
                    return;
                }
                // Add the new post
                var newPost = new Domain.Entities.Post
                {
                    PostId = post.PostId,
                    AuthorId = post.AuthorId,
                    Privacy = post.Privacy,
                };
                var isAdded = await _postRepo.AddPostAsync(newPost);
                if (!isAdded)
                {
                    throw new Exception($"Failed to add post with ID {post.PostId}.");
                }
            }
            else if (post.EventType == EventType.Delete)
            {
                // Delete the post
                await _postRepo.DeletePostAsync(post.PostId);
                // Delete all comments related to this post
                await _commentRepo.DeleteByPostIdAsync(post.PostId);
            }
            else
            {
                throw new ArgumentException("Unsupported post event type.", nameof(post.EventType));
            }

            // Check if the post already exists
        }
    }
}
