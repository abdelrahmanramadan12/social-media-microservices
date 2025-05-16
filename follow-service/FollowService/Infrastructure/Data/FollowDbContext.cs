using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infrastructure.Data
{
    public class FollowDbContext : DbContext
    {
        public FollowDbContext(DbContextOptions<FollowDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToCollection("users");
                entity.Property(e => e.UserId).HasElementName("user_id");
                entity.Property(e => e.Followers).HasElementName("followers");
                entity.Property(e => e.Following).HasElementName("following");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
