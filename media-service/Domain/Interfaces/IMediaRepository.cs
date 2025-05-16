using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMediaRepository
    {
        Task<Media> AddAsync(IFormFile file, string? description, string? tags);
        Task<Media?> GetByIdAsync(int id);
        Task<IEnumerable<Media>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
        Task<string> SaveFileAsync(IFormFile file, string uploadsFolder);
    }
}
