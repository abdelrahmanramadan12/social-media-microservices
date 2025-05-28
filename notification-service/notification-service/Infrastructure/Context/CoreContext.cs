using Domain.CoreEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class CoreContext(DbContextOptions<CoreContext> options) : DbContext(options)
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follows> Follows { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Follows>().ToTable("Follows");
            modelBuilder.Entity<Messages>().ToTable("Messages");
            modelBuilder.Entity<Reaction>().ToTable("Reactions");
        }
    }
}
