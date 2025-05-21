using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infrastructure.Data
{
    public class FollowDbContext : DbContext
    {
        public FollowDbContext(DbContextOptions<FollowDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToCollection("users");
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.ToCollection("follows");
                entity.Property(e => e.FollowerId).HasElementName("follower_id");
                entity.Property(e => e.FollowingId).HasElementName("following_id");
                entity.Property(e => e.FollowedAt).HasElementName("followed_at");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
