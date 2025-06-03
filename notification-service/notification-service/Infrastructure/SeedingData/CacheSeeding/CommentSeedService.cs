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

        private List<CachedCommentsNotification> GenerateSampleCommentsData()
        {
            var utcNow = DateTime.UtcNow;

            return new List<CachedCommentsNotification>
            {
                // User 1
                new CachedCommentsNotification
                {
                    UserId = "user1",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u1_1",
                                UserId = "user1",
                                ProfileImageUrls = "https://example.com/user1.jpg",
                                UserNames = "John Doe"
                            },
                            PostId = "post1",
                            CommentId = "comment1",
                            Content = "This is a great post!"
                        },
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u1_1",
                                UserId = "user1",
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
                new CachedCommentsNotification
                {
                    UserId = "user2",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u2_1",
                                UserId = "user2",
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
                new CachedCommentsNotification
                {
                    UserId = "user3",
                    CommnetDetails = new List<CommnetNotificationDetails>()
                },
                
                // User 4
                new CachedCommentsNotification
                {
                    UserId = "user4",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u4_1",
                                UserId = "user4",
                                ProfileImageUrls = "https://example.com/user4.jpg",
                                UserNames = "Alice Johnson"
                            },
                            PostId = "post1",
                            CommentId = "comment4",
                            Content = "Thanks for sharing this"
                        },
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u4_1",
                                UserId = "user4",
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
                new CachedCommentsNotification
                {
                    UserId = "user5",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u5_1",
                                UserId = "user5",
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
                new CachedCommentsNotification
                {
                    UserId = "user6",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u6_1",
                                UserId = "user6",
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
                new CachedCommentsNotification
                {
                    UserId = "user7",
                    CommnetDetails = new List<CommnetNotificationDetails>()
                },
                
                // User 8
                new CachedCommentsNotification
                {
                    UserId = "user8",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u8_1",
                                UserId = "user8",
                                ProfileImageUrls = "https://example.com/user8.jpg",
                                UserNames = "Diana Evans"
                            },
                            PostId = "post3",
                            CommentId = "comment8",
                            Content = "I have a different opinion about this"
                        },
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u8_1",
                                UserId = "user8",
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
                new CachedCommentsNotification
                {
                    UserId = "user9",
                    CommnetDetails = new List<CommnetNotificationDetails>
                    {
                        new CommnetNotificationDetails
                        {
                            User = new UserSkeleton
                            {
                                Id = "u9_1",
                                UserId = "user9",
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
                new CachedCommentsNotification
                {
                    UserId = "user10",
                    CommnetDetails = new List<CommnetNotificationDetails>()
                }
            };
        }
        }
    
}