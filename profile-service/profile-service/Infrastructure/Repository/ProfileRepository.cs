using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ProfileRepository : IProfileRepository
    {

        private readonly ProfileDbContext _context;

        public ProfileRepository(ProfileDbContext context)
        {
            _context = context;
        }

        public async Task<Profile?> GetByUserIdAsync(string userId)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Profile?> GetByUserNameAsync(string userName)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserName.ToLower() == userName.ToLower());
        }

        public async Task<List<Profile>> SearchByUserNameAsync(string userName, int pageNumber, int pageSize)
        {
            return await _context.Profiles
                .Where(p => EF.Functions.Like(p.UserName.ToLower(), $"%{userName.ToLower()}%"))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<Profile?> GetByUserIdMinAsync(string userId)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Profile?> GetByUserNameMinAsync(string userName)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserName.ToLower() == userName.ToLower());
        }

        public async Task<List<Profile>> GetUsersByIdsAsync(List<string> userIds)
        {
            return await _context.Profiles
                .Where(p => userIds.Contains(p.UserId))
                .ToListAsync();
        }

        public async Task AddAsync(Profile profile)
        {
            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Profile profile)
        {
            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Profiles.AnyAsync(p => p.UserId == userId);
        }
        public async Task DeleteAsync(string userId)
        {
            var profile = await _context.Profiles.FindAsync(userId);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
            }
        }

        public async Task IncrementFollowingCountAsync(string userId)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null)
            {
                profile.NoFollowing++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementFollowerCountAsync(string userId)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null)
            {
                profile.NoFollowers++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementFollowingCountAsync(string userId)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null && profile.NoFollowing > 0)
            {
                profile.NoFollowing--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementFollowerCountAsync(string userId)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null && profile.NoFollowers > 0)
            {
                profile.NoFollowers--;
                await _context.SaveChangesAsync();
            }
        }

    }
}
