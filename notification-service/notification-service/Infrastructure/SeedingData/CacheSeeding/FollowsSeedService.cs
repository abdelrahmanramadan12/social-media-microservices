using Domain.CacheEntities;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.SeedingData.CacheSeeding
{
    public class RedisFollowsSeeder
    {
        private readonly IDatabase _redisDb;

        public RedisFollowsSeeder(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task SeedInitialFollowsDataAsync()
        {
            var followsData =GenerateSampleFollowsData();

            foreach (var cachedFollow in followsData)
            {
                try
                {
                    var key = $"cachedfollowed:{cachedFollow.UserId}";
                    await _redisDb.StringSetAsync(
                        key,
                        JsonSerializer.Serialize(cachedFollow),
                        TimeSpan.FromDays(30) 
                    );
                    await _redisDb.SetAddAsync("cachedfollowed", key);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
              
        }

        private List<CachedFollowed> GenerateSampleFollowsData()
        {
            var utcNow = DateTime.UtcNow;

            return new List<CachedFollowed>
    {
        // User 1 (from your example)
        new CachedFollowed
        {
            UserId = "user1",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f1_1",
                    UsersId = "user101",
                    ProfileImageUrls = "https://example.com/user101.jpg",
                    UserNames = "Alex Johnson",
                    Seen = false,
                    CreatedAt = utcNow.AddDays(-2)
                },
                new UserMostUsedData
                {
                    Id = "f1_2",
                    UsersId = "user102",
                    ProfileImageUrls = "https://example.com/user102.jpg",
                    UserNames = "Maria Garcia",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-5)
                }
            }
        },
        
        // User 2
        new CachedFollowed
        {
            UserId = "user2",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f2_1",
                    UsersId = "user103",
                    ProfileImageUrls = "https://example.com/user103.jpg",
                    UserNames = "James Smith",
                    Seen = false,
                    CreatedAt = utcNow.AddDays(-1)
                }
            }
        },
        
        // User 3 (No followers)
        new CachedFollowed
        {
            UserId = "user3",
            Followers = new List<UserMostUsedData>()
        },
        
        // User 4
        new CachedFollowed
        {
            UserId = "user4",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f4_1",
                    UsersId = "user104",
                    ProfileImageUrls = "https://example.com/user104.jpg",
                    UserNames = "Emma Wilson",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-7)
                },
                new UserMostUsedData
                {
                    Id = "f4_2",
                    UsersId = "user105",
                    ProfileImageUrls = "https://example.com/user105.jpg",
                    UserNames = "Liam Brown",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-3)
                }
            }
        },
        
        // User 5
        new CachedFollowed
        {
            UserId = "user5",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f5_1",
                    UsersId = "user106",
                    ProfileImageUrls = "https://example.com/user106.jpg",
                    UserNames = "Olivia Davis",
                    Seen = false,
                    CreatedAt = utcNow.AddHours(-12)
                }
            }
        },
        
        // User 6
        new CachedFollowed
        {
            UserId = "user6",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f6_1",
                    UsersId = "user107",
                    ProfileImageUrls = "https://example.com/user107.jpg",
                    UserNames = "Noah Martinez",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-10)
                },
                new UserMostUsedData
                {
                    Id = "f6_2",
                    UsersId = "user108",
                    ProfileImageUrls = "https://example.com/user108.jpg",
                    UserNames = "Sophia Anderson",
                    Seen = false,
                    CreatedAt = utcNow.AddDays(-4)
                },
                new UserMostUsedData
                {
                    Id = "f6_3",
                    UsersId = "user109",
                    ProfileImageUrls = "https://example.com/user109.jpg",
                    UserNames = "William Taylor",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-6)
                }
            }
        },
        
        // User 7
        new CachedFollowed
        {
            UserId = "user7",
            Followers = new List<UserMostUsedData>()
        },
        
        // User 8
        new CachedFollowed
        {
            UserId = "user8",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f8_1",
                    UsersId = "user110",
                    ProfileImageUrls = "https://example.com/user110.jpg",
                    UserNames = "Ava Thomas",
                    Seen = false,
                    CreatedAt = utcNow.AddDays(-9)
                }
            }
        },
        
        // User 9
        new CachedFollowed
        {
            UserId = "user9",
            Followers = new List<UserMostUsedData>
            {
                new UserMostUsedData
                {
                    Id = "f9_1",
                    UsersId = "user111",
                    ProfileImageUrls = "https://example.com/user111.jpg",
                    UserNames = "Benjamin Lee",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-8)
                },
                new UserMostUsedData
                {
                    Id = "f9_2",
                    UsersId = "user112",
                    ProfileImageUrls = "https://example.com/user112.jpg",
                    UserNames = "Mia Hernandez",
                    Seen = true,
                    CreatedAt = utcNow.AddDays(-11)
                }
            }
        },
        
        // User 10 (No followers)
        new CachedFollowed
        {
            UserId = "user10",
            Followers = new List<UserMostUsedData>()
        }
    };
        }
    }
}