using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPostRepository
    {
        // Create 
        Post CreatePost (Post post);

        // Read
        Post GetPost (string postId);

        // Read List
        List<Post> GetUserPosts (string userId, int pageSize, string? cursorPostId);

        // Update
        Post UpdatePost(string postId, Post newPost);

        // Delete
        bool DeletePost (string postId);
    }
}
