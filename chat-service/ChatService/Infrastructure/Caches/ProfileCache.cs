using Application.Abstractions;
using Application.DTOs;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Caches
{
    public class ProfileCache : IProfileCache
    {
        private readonly IDatabase _db;
        private const string Prefix = "user_profile:";

        public ProfileCache(IDatabase db)
        {
            _db = db;
        }

        public async Task AddProfilesAsync(Dictionary<string, ProfileDTO> profiles)
        {
            var entries = profiles.Select(kvp =>
                new KeyValuePair<RedisKey, RedisValue>(
                    $"user_profile:{kvp.Key}",
                    JsonSerializer.Serialize(kvp.Value)
                )
            ).ToArray();

            await _db.StringSetAsync(entries);
        }

        public async Task<List<ProfileDTO>> GetProfilesAsync(IEnumerable<string> userIds)
        {
            var keys = userIds.Select(id => (RedisKey)$"user_profile:{id}").ToArray();
            var results = await _db.StringGetAsync(keys);

            return results
                .Where(r => r.HasValue)
                .Select(r => JsonSerializer.Deserialize<ProfileDTO>(r!))
                .Where(p => p != null)
                .ToList()!;
        }
    }
}
