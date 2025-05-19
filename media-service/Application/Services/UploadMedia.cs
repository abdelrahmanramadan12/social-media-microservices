using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UploadMediaService(IMediaRepository mediaRepository) : IUploadMediaService
    {
        private readonly IMediaRepository _mediaRepository = mediaRepository;

        public async Task<string> UploadMediaAsync(IFormFile file, string? description)
        {
            // Assuming you have a method to upload the media to the database or storage
            // For example, you might have a method like this:
            var media = await _mediaRepository.AddAsync(file, description);
            return media.Url ?? throw new Exception("Sorry We could not upload your image at the moment .. please try again later !");
        }

        public async Task<bool> DeleteMediaAsync(Guid id)
        {
            // Assuming you have a method to delete the media from the database or storage
            // For example, you might have a method like this:
            bool isDeleted = await _mediaRepository.DeleteAsync(id);
            if (!isDeleted)
            {
                throw new Exception("Sorry We could not delete your image at the moment .. please try again later !");
            }

            return isDeleted;
        }
    }
}
