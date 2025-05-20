using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infrastructure.Data
{
    public class CommentContext:DbContext
    {
        public CommentContext(DbContextOptions<CommentContext> options) : base(options)
        {
        }
        public DbSet<Comment> Comments { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>().ToCollection("comments");

        }
    }

}
