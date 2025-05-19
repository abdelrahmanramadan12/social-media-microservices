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
        Task<Media> AddAsync(IFormFile file, string? description);
        Task<string> GetMediaUrlById(Guid MediaId);
        Task<Media> GetMediaByUrl(string MediaUrl);
        Task<bool> DeleteAsync(Guid id);
    }
}
