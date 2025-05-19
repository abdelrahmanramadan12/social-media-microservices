using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Infrastructure.Repository
{
    public class MediaRepository : IMediaRepository
    {
        public Task<Media> AddAsync(IFormFile file, string? description)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File cannot be null");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty", nameof(description));
            }

            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {

            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            throw new NotImplementedException();
        }

        public Task<string> GetMediaUrlById(Guid MediaId)
        {
            if (MediaId == Guid.Empty)
            {
                throw new ArgumentException("MediaId cannot be empty", nameof(MediaId));
            }


            throw new NotImplementedException();
        }

        public Task<Media> GetMediaByUrl(string MediaUrl)
        {
            if (string.IsNullOrWhiteSpace(MediaUrl))
            {
                throw new ArgumentException("MediaUrl cannot be null or empty", nameof(MediaUrl));
            }
            throw new NotImplementedException();
        }


        //public Task<bool>
        public async Task<bool> EditMediaAsync([FromBody] Media? newMedia, string MediaUrl)
        {
            // Assuming you have a method to edit the media in the database or storage
            // For example, you might have a method like this:
            if (string.IsNullOrWhiteSpace(MediaUrl))
            {
                throw new ArgumentException("MediaUrl cannot be null or empty", nameof(MediaUrl));
            }

            Media? Media = await GetMediaByUrl(MediaUrl) ?? throw new Exception("Media not found");
            if (newMedia == null)
            {
                throw new ArgumentNullException(nameof(newMedia), "New media cannot be null");
            }

            Media.Url = newMedia.Url;

            throw new NotImplementedException();

        }
    }
}
