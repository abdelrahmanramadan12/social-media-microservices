using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    internal class MediaRepository : IMediaRepository
    {
        public Task<Media> AddAsync(IFormFile file, string? description, string? tags)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Media>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Media?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveFileAsync(IFormFile file, string uploadsFolder)
        {
            throw new NotImplementedException();
        }
    }
}
