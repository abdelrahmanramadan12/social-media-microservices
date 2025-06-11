using Domain.CoreEntities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Infrastructure.SeedingData.mongdbSeeding
{
    public class MongoFollowsSeeder
    {
        private readonly IMongoCollection<Follows> _followsCollection;

        public MongoFollowsSeeder(IMongoDatabase database)
        {
            _followsCollection = database.GetCollection<Follows>("Follows");
        }

        public async Task SeedInitialFollowsDataAsync()
        {
            // Check if data already exists
            var count = await _followsCollection.CountDocumentsAsync(FilterDefinition<Follows>.Empty);
            if (count > 0)
            {
                return; // Data already seeded
            }

            var followsData = GenerateSampleFollowsData();
            await _followsCollection.InsertManyAsync(followsData);
        }

        private List<Follows> GenerateSampleFollowsData()
        {
            var utcNow = DateTime.UtcNow;

            return new List<Follows>
            {
                // User 1
                new Follows
                {
                    MyId = "user1",
                    FollowersId = new List<string> { "user101", "user102" },
                    FollowsNotifReadByAuthor = new List<string> { "user102" } // Maria Garcia was seen
                },
                
                // User 2
                new Follows
                {
                    MyId = "user2",
                    FollowersId = new List<string> { "user103" },
                    FollowsNotifReadByAuthor = new List<string>() // James Smith not seen
                },
                
                // User 3 (No followers)
                new Follows
                {
                    MyId = "user3",
                    FollowersId = new List<string>(),
                    FollowsNotifReadByAuthor = new List<string>()
                },
                
                // User 4
                new Follows
                {
                    MyId = "user4",
                    FollowersId = new List<string> { "user104", "user105" },
                    FollowsNotifReadByAuthor = new List<string> { "user104", "user105" } // Both seen
                },
                
                // User 5
                new Follows
                {
                    MyId = "user5",
                    FollowersId = new List<string> { "user106" },
                    FollowsNotifReadByAuthor = new List<string>() // Olivia Davis not seen
                },
                
                // User 6
                new Follows
                {
                    MyId = "user6",
                    FollowersId = new List<string> { "user107", "user108", "user109" },
                    FollowsNotifReadByAuthor = new List<string> { "user107", "user109" } // Noah and William seen
                },
                
                // User 7
                new Follows
                {
                    MyId = "user7",
                    FollowersId = new List<string>(),
                    FollowsNotifReadByAuthor = new List<string>()
                },
                
                // User 8
                new Follows
                {
                    MyId = "user8",
                    FollowersId = new List<string> { "user110" },
                    FollowsNotifReadByAuthor = new List<string>() // Ava Thomas not seen
                },
                
                // User 9
                new Follows
                {
                    MyId = "user9",
                    FollowersId = new List<string> { "user111", "user112" },
                    FollowsNotifReadByAuthor = new List<string> { "user111", "user112" } // Both seen
                },
                
                // User 10 (No followers)
                new Follows
                {
                    MyId = "user10",
                    FollowersId = new List<string>(),
                    FollowsNotifReadByAuthor = new List<string>()
                }
            };
        }
    }
}