using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using StackExchange.Redis;
using System.Text.Json;


namespace Infrastructure.SeedingData.CacheSeeding
{

        public class RedisCommentsSeeder
        {
            private readonly IDatabase _redisDb;

            public RedisCommentsSeeder(IConnectionMultiplexer redis)
            {
                _redisDb = redis.GetDatabase();
            }

            public async Task SeedInitialCommentsDataAsync()
            {
                var commentsData = GenerateSampleCommentsData();

                foreach (var cachedComment in commentsData)
                {
                    try
                    {
                        var key = $"cachedcomments:{cachedComment.UserId}";
                        await _redisDb.StringSetAsync(
                            key,
                            JsonSerializer.Serialize(cachedComment),
                            TimeSpan.FromDays(30)
                        );
                        await _redisDb.SetAddAsync("cachedcomments", key);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        private List<CachedComments> GenerateSampleCommentsData()
        {
            var utcNow = DateTime.UtcNow;

            return new List<CachedComments>
            {
                // User 1
                new CachedComments
                {
                    UserId = "user1",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u1_1",
                                UsersId = "user1",
                                ProfileImageUrls = "https://example.com/user1.jpg",
                                UserNames = "John Doe"
                            },
                            PostId = "post1",
                            CommentId = "comment1",
                            Content = "This is a great post!"
                        },
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u1_1",
                                UsersId = "user1",
                                ProfileImageUrls = "https://example.com/user1.jpg",
                                UserNames = "John Doe"
                            },
                            PostId = "post2",
                            CommentId = "comment2",
                            Content = "I completely agree with this"
                        }
                    }
                },
                
                // User 2
                new CachedComments
                {
                    UserId = "user2",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u2_1",
                                UsersId = "user2",
                                ProfileImageUrls = "https://example.com/user2.jpg",
                                UserNames = "Jane Smith"
                            },
                            PostId = "post3",
                            CommentId = "comment3",
                            Content = "Interesting perspective"
                        }
                    }
                },
                
                // User 3 (No comments)
                new CachedComments
                {
                    UserId = "user3",
                    CommnetDetails = new List<CommnetDetails>()
                },
                
                // User 4
                new CachedComments
                {
                    UserId = "user4",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u4_1",
                                UsersId = "user4",
                                ProfileImageUrls = "https://example.com/user4.jpg",
                                UserNames = "Alice Johnson"
                            },
                            PostId = "post1",
                            CommentId = "comment4",
                            Content = "Thanks for sharing this"
                        },
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u4_1",
                                UsersId = "user4",
                                ProfileImageUrls = "https://example.com/user4.jpg",
                                UserNames = "Alice Johnson"
                            },
                            PostId = "post4",
                            CommentId = "comment5",
                            Content = "This changed my mind about the topic"
                        }
                    }
                },
                
                // User 5
                new CachedComments
                {
                    UserId = "user5",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u5_1",
                                UsersId = "user5",
                                ProfileImageUrls = "https://example.com/user5.jpg",
                                UserNames = "Bob Brown"
                            },
                            PostId = "post2",
                            CommentId = "comment6",
                            Content = "Can you explain this part more?"
                        }
                    }
                },
                
                // User 6
                new CachedComments
                {
                    UserId = "user6",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u6_1",
                                UsersId = "user6",
                                ProfileImageUrls = "https://example.com/user6.jpg",
                                UserNames = "Charlie Davis"
                            },
                            PostId = "post5",
                            CommentId = "comment7",
                            Content = "First comment here!"
                        }
                    }
                },
                
                // User 7 (No comments)
                new CachedComments
                {
                    UserId = "user7",
                    CommnetDetails = new List<CommnetDetails>()
                },
                
                // User 8
                new CachedComments
                {
                    UserId = "user8",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u8_1",
                                UsersId = "user8",
                                ProfileImageUrls = "https://example.com/user8.jpg",
                                UserNames = "Diana Evans"
                            },
                            PostId = "post3",
                            CommentId = "comment8",
                            Content = "I have a different opinion about this"
                        },
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u8_1",
                                UsersId = "user8",
                                ProfileImageUrls = "https://example.com/user8.jpg",
                                UserNames = "Diana Evans"
                            },
                            PostId = "post6",
                            CommentId = "comment9",
                            Content = "This is exactly what I needed to see today"
                        }
                    }
                },
                
                // User 9
                new CachedComments
                {
                    UserId = "user9",
                    CommnetDetails = new List<CommnetDetails>
                    {
                        new CommnetDetails
                        {
                            User = new UserMostUsedData
                            {
                                Id = "u9_1",
                                UsersId = "user9",
                                ProfileImageUrls = "https://example.com/user9.jpg",
                                UserNames = "Ethan Wilson"
                            },
                            PostId = "post7",
                            CommentId = "comment10",
                            Content = "Looking forward to more content like this"
                        }
                    }
                },
                
                // User 10 (No comments)
                new CachedComments
                {
                    UserId = "user10",
                    CommnetDetails = new List<CommnetDetails>()
                }
            };
        }
        }
    
}