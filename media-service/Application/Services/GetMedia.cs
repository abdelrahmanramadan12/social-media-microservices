using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GetMediaService(IMediaRepository mediaRepository) : IGetMediaService
    {
        private readonly IMediaRepository _mediaRepository = mediaRepository;
        public async Task<string> GetMediaURL(Guid MediaId)
        {
            // Assuming you have a method to get the media URL from the database or storage
            // For example, you might have a method like this:


            string mediaUrl = await _mediaRepository.GetMediaUrlById(MediaId);

            return mediaUrl ?? throw new Exception("Media not found");

        }

        public async Task<Media?> GetMediaAsync(string MediaUrl)
        {
            // Assuming you have a method to get the media from the database or storage
            // For example, you might have a method like this:
            Media? Media = await _mediaRepository.GetMediaByUrl(MediaUrl);
            return Media ?? throw new Exception("Media not found");
        }

    }
}
