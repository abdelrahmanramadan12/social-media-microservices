using Domain.CacheEntities;
using Domain.CacheEntities.Reactions;
using Domain.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Infrastructure.SeedingData.CacheSeeding
{
    public class RedisReactionsSeeder
    {
        private readonly IDatabase _redisDb;

        public RedisReactionsSeeder(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task SeedInitialReactionsDataAsync()
        {
            var reactionsData = GenerateSampleReactionsData();

            foreach (var cachedReaction in reactionsData)
            {
                try
                {
                    var key = $"cachedreactions:{cachedReaction.AuthorId}";
                    await _redisDb.StringSetAsync(
                        key,
                        JsonSerializer.Serialize(cachedReaction),
                        TimeSpan.FromDays(30)
                    );
                    await _redisDb.SetAddAsync("cachedreactions", key);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private List<CachedReactions> GenerateSampleReactionsData()
        {
            return new List<CachedReactions>
        {
            // User 1 - Author with reactions on posts and comments
            new CachedReactions
            {
                AuthorId = "user1",
                ReactionsOnPosts = new List<ReactionPostDetails>
                {
                    new ReactionPostDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u1_1",
                            UsersId = "user101",
                            ProfileImageUrls = "https://example.com/user101.jpg",
                            UserNames = "Alex Johnson"
                        },
                        PostId = "post1",
                        ReactionId = "react1",
                        PostContent = "Check out my new post!",
                        ReactionType = ReactionType.Like
                    },
                    new ReactionPostDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u1_2",
                            UsersId = "user102",
                            ProfileImageUrls = "https://example.com/user102.jpg",
                            UserNames = "Maria Garcia"
                        },
                        PostId = "post2",
                        ReactionId = "react2",
                        PostContent = "My vacation photos",
                        ReactionType = ReactionType.Love
                    }
                },
                ReactionsOnComments = new List<ReactionCommentDetails>
                {
                    new ReactionCommentDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u1_3",
                            UsersId = "user103",
                            ProfileImageUrls = "https://example.com/user103.jpg",
                            UserNames = "James Smith"
                        },
                        CommentId = "comment1",
                        ReactionId = "react3",
                        CommentContent = "Great post!",
                        ReactionType = ReactionType.Wow
                    }
                }
            },
                
            // User 2 - Only post reactions
            new CachedReactions
            {
                AuthorId = "user2",
                ReactionsOnPosts = new List<ReactionPostDetails>
                {
                    new ReactionPostDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u2_1",
                            UsersId = "user104",
                            ProfileImageUrls = "https://example.com/user104.jpg",
                            UserNames = "Emma Wilson"
                        },
                        PostId = "post3",
                        ReactionId = "react4",
                        PostContent = "Tech news update",
                        ReactionType = ReactionType.Love
                    }
                },
                ReactionsOnComments = new List<ReactionCommentDetails>()
            },
                
            // User 3 - Only comment reactions
            new CachedReactions
            {
                AuthorId = "user3",
                ReactionsOnPosts = new List<ReactionPostDetails>(),
                ReactionsOnComments = new List<ReactionCommentDetails>
                {
                    new ReactionCommentDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u3_1",
                            UsersId = "user105",
                            ProfileImageUrls = "https://example.com/user105.jpg",
                            UserNames = "Liam Brown"
                        },
                        CommentId = "comment2",
                        ReactionId = "react5",
                        CommentContent = "Interesting perspective",
                        ReactionType = ReactionType.Love
                    }
                }
            },
                
            // User 4 - Multiple reactions of different types
            new CachedReactions
            {
                AuthorId = "user4",
                ReactionsOnPosts = new List<ReactionPostDetails>
                {
                    new ReactionPostDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u4_1",
                            UsersId = "user106",
                            ProfileImageUrls = "https://example.com/user106.jpg",
                            UserNames = "Olivia Davis"
                        },
                        PostId = "post4",
                        ReactionId = "react6",
                        PostContent = "Cooking recipe",
                        ReactionType = ReactionType.Like
                    },
                    new ReactionPostDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u4_2",
                            UsersId = "user107",
                            ProfileImageUrls = "https://example.com/user107.jpg",
                            UserNames = "Noah Martinez"
                        },
                        PostId = "post5",
                        ReactionId = "react7",
                        PostContent = "Travel tips",
                        ReactionType = ReactionType.Wow
                    }
                },
                ReactionsOnComments = new List<ReactionCommentDetails>
                {
                    new ReactionCommentDetails
                    {
                        User = new UserMostUsedData
                        {
                            Id = "u4_3",
                            UsersId = "user108",
                            ProfileImageUrls = "https://example.com/user108.jpg",
                            UserNames = "Sophia Anderson"
                        },
                        CommentId = "comment3",
                        ReactionId = "react8",
                        CommentContent = "Thanks for sharing",
                        ReactionType = ReactionType.Like
                    }
                }
            },
                
            // User 5 - No reactions
            new CachedReactions
            {
                AuthorId = "user5",
                ReactionsOnPosts = new List<ReactionPostDetails>(),
                ReactionsOnComments = new List<ReactionCommentDetails>()
            }
        };
        }
    }
}
