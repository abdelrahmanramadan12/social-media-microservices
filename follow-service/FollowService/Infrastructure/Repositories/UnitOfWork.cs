

using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;

namespace Xcourse.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FollowDbContext _context;

        public IRepository<User> Users { get; }
        public IRepository<Follow> Follows { get; }

        public UnitOfWork(FollowDbContext context)
        {
            _context = context;

            Users = new BaseRepository<User>(_context);
            Follows = new BaseRepository<Follow>(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}