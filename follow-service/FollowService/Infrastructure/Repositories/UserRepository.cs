using Application.Abstractions;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("users");
        }

        public async Task<User> FindAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var user = await _users.FindAsync(filter);
            return await user.FirstOrDefaultAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _users.DeleteOneAsync(filter);
            return;
        }
    }
}
