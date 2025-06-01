using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.CoreEntities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;


namespace Infrastructure.SeedingData.mongdbSeeding
{
    public class MongoCommentsSeeder
    {
        private readonly IMongoCollection<Comment> _commentsCollection;

        public MongoCommentsSeeder(IMongoDatabase database)
        {
            _commentsCollection = database.GetCollection<Comment>("Comments");
        }

        public async Task SeedInitialCommentsDataAsync()
        {
            // Check if data already exists
            var count = await _commentsCollection.CountDocumentsAsync(FilterDefinition<Comment>.Empty);
            if (count > 0)
            {
                return; // Data already seeded
            }

            var commentsData = GenerateSampleCommentsData();
            await _commentsCollection.InsertManyAsync(commentsData);
        }

        private List<Comment> GenerateSampleCommentsData()
        {
            return new List<Comment>
        {
            // User 1
            new Comment
            {
                AuthorId = "user1",
                PostId = "post1",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user1", "comment1" } }
                },
                CommentNotifReadByAuthor = new List<string> { "comment1" }
            },
            new Comment
            {
                AuthorId = "user1",
                PostId = "post2",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user1", "comment2" } }
                },
                CommentNotifReadByAuthor = new List<string>()
            },
                
            // User 2
            new Comment
            {
                AuthorId = "user2",
                PostId = "post3",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user2", "comment3" } }
                },
                CommentNotifReadByAuthor = new List<string> { "comment3" }
            },
                
            // User 4
            new Comment
            {
                AuthorId = "user4",
                PostId = "post1",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user4", "comment4" } }
                },
                CommentNotifReadByAuthor = new List<string>()
            },
            new Comment
            {
                AuthorId = "user4",
                PostId = "post4",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user4", "comment5" } }
                },
                CommentNotifReadByAuthor = new List<string> { "comment5" }
            },
                
            // User 5
            new Comment
            {
                AuthorId = "user5",
                PostId = "post2",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user5", "comment6" } }
                },
                CommentNotifReadByAuthor = new List<string>()
            },
                
            // User 6
            new Comment
            {
                AuthorId = "user6",
                PostId = "post5",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user6", "comment7" } }
                },
                CommentNotifReadByAuthor = new List<string> { "comment7" }
            },
                
            // User 8
            new Comment
            {
                AuthorId = "user8",
                PostId = "post3",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user8", "comment8" } }
                },
                CommentNotifReadByAuthor = new List<string>()
            },
            new Comment
            {
                AuthorId = "user8",
                PostId = "post6",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user8", "comment9" } }
                },
                CommentNotifReadByAuthor = new List<string> { "comment9" }
            },
                
            // User 9
            new Comment
            {
                AuthorId = "user9",
                PostId = "post7",
                UserID_CommentId = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "user9", "comment10" } }
                },
                CommentNotifReadByAuthor = new List<string>()
            }
        };
        }
    }
}
