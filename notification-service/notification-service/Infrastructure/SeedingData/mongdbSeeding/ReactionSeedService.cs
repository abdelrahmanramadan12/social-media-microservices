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
    public class MongoReactionsSeeder
    {
        private readonly IMongoCollection<Reaction> _reactionsCollection;

        public MongoReactionsSeeder(IMongoDatabase database)
        {
            _reactionsCollection = database.GetCollection<Reaction>("Reaction");
        }

        public async Task SeedInitialReactionsDataAsync()
        {
            // Check if data already exists
            var count = await _reactionsCollection.CountDocumentsAsync(FilterDefinition<Reaction>.Empty);
            if (count > 0)
            {
                return; // Data already seeded
            }

            var reactionsData = GenerateSampleReactionsData();
            await _reactionsCollection.InsertManyAsync(reactionsData);
        }

        private List<Reaction> GenerateSampleReactionsData()
        {
            return new List<Reaction>
        {
            // User 1
            new Reaction
            {
                AuthorId = "user1",
                ReactionsOnPostId = new List<string> { "react1", "react2" },
                ReactionsOnCommentId = new List<string> { "react3" },
                PostReactionsNotifReadByAuthor = new List<string> { "react1" },
                CommentReactionsNotifReadByAuthor = new List<string>()
            },
                
            // User 2
            new Reaction
            {
                AuthorId = "user2",
                ReactionsOnPostId = new List<string> { "react4" },
                ReactionsOnCommentId = new List<string>(),
                PostReactionsNotifReadByAuthor = new List<string>(),
                CommentReactionsNotifReadByAuthor = new List<string>()
            },
                
            // User 3
            new Reaction
            {
                AuthorId = "user3",
                ReactionsOnPostId = new List<string>(),
                ReactionsOnCommentId = new List<string> { "react5" },
                PostReactionsNotifReadByAuthor = new List<string>(),
                CommentReactionsNotifReadByAuthor = new List<string> { "react5" }
            },
                
            // User 4
            new Reaction
            {
                AuthorId = "user4",
                ReactionsOnPostId = new List<string> { "react6", "react7" },
                ReactionsOnCommentId = new List<string> { "react8" },
                PostReactionsNotifReadByAuthor = new List<string> { "react6" },
                CommentReactionsNotifReadByAuthor = new List<string>()
            },
                
            // User 5 (No reactions)
            new Reaction
            {
                AuthorId = "user5",
                ReactionsOnPostId = new List<string>(),
                ReactionsOnCommentId = new List<string>(),
                PostReactionsNotifReadByAuthor = new List<string>(),
                CommentReactionsNotifReadByAuthor = new List<string>()
            }
        };
        }
    }
}
