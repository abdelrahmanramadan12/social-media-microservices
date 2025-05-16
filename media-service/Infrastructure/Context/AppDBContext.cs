using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;

    }
}
