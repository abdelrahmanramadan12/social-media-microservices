using Domain.CoreEntities;
using MongoDB.Driver;


namespace Infrastructure.SeedingData.mongdbSeeding
{
    public class MongoCommentsSeeder
    {
        private readonly IMongoCollection<Comments> _commentsCollection;

        public MongoCommentsSeeder(IMongoDatabase database)
        {
            _commentsCollection = database.GetCollection<Comments>("Comments");
        }

        public async Task SeedInitialCommentsDataAsync()
        {
            // Check if data already exists
            var count = await _commentsCollection.CountDocumentsAsync(FilterDefinition<Comments>.Empty);
            if (count > 0)
            {
                return; // Data already seeded
            }

            var commentsData = GenerateSampleCommentsData();
            await _commentsCollection.InsertManyAsync(commentsData);
        }

        private List<Comments> GenerateSampleCommentsData()
        {
            return new List<Comments>
    {
        // User 1
        new Comments
        {
            PostAuthorId = "user1",
            PostId = "post1",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user1", new List<string> { "comment1" } }
            },
            CommentNotifReadByAuthor = new List<string> { "comment1" }
        },
        new Comments
        {
            PostAuthorId = "user1",
            PostId = "post2",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user1", new List<string> { "comment2" } }
            },
            CommentNotifReadByAuthor = new List<string>()
        },

        // User 2
        new Comments
        {
            PostAuthorId = "user2",
            PostId = "post3",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user2", new List<string> { "comment3" } }
            },
            CommentNotifReadByAuthor = new List<string> { "comment3" }
        },

        // User 4
        new Comments
        {
            PostAuthorId = "user4",
            PostId = "post1",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user4", new List<string> { "comment4" } }
            },
            CommentNotifReadByAuthor = new List<string>()
        },
        new Comments
        {
            PostAuthorId = "user4",
            PostId = "post4",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user4", new List<string> { "comment5" } }
            },
            CommentNotifReadByAuthor = new List<string> { "comment5" }
        },

        // User 5
        new Comments
        {
            PostAuthorId = "user5",
            PostId = "post2",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user5", new List<string> { "comment6" } }
            },
            CommentNotifReadByAuthor = new List<string>()
        },

        // User 6
        new Comments
        {
            PostAuthorId = "user6",
            PostId = "post5",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user6", new List<string> { "comment7" } }
            },
            CommentNotifReadByAuthor = new List<string> { "comment7" }
        },

        // User 8
        new Comments
        {
            PostAuthorId = "user8",
            PostId = "post3",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user8", new List<string> { "comment8" } }
            },
            CommentNotifReadByAuthor = new List<string>()
        },
        new Comments
        {
            PostAuthorId = "user8",
            PostId = "post6",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user8", new List<string> { "comment9" } }
            },
            CommentNotifReadByAuthor = new List<string> { "comment9" }
        },

        // User 9
        new Comments
        {
            PostAuthorId = "user9",
            PostId = "post7",
            UserID_CommentIds = new Dictionary<string, List<string>>
            {
                { "user9", new List<string> { "comment10" } }
            },
            CommentNotifReadByAuthor = new List<string>()
        }
    };
        }

    }
}
