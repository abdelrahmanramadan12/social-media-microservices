using Domain.IRepository;
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
            if (post.EventType == postEventType.PostAdded)
            {


                var existingPost = _postRepo.GetPostByIdAsync(post.Data.PostId);
                if (existingPost != null)
                {
                    return;
                }
                // Add the new post
                var newPost = new Domain.Entities.Post
                {
                    Id = post.Data.PostId,
                    AuthorId = post.Data.PostAuthorId,
                    Privacy = post.Data.Privacy,
                };
                var isAdded = await _postRepo.AddPostAsync(newPost);
                if (!isAdded)
                {
                    throw new Exception($"Failed to add post with ID {post.Data.PostId}.");
                }
            }
            else if (post.EventType == postEventType.PostDeleted)
            {
                // Delete the post
                await _postRepo.DeletePostAsync(post.Data.PostId);
                // Delete all comments related to this post
                await _commentRepo.DeleteByPostIdAsync(post.Data.PostId);
            }
            else
            {
                throw new ArgumentException("Unsupported post event type.", nameof(post.EventType));
            }

            // Check if the post already exists
        }
    }
}
