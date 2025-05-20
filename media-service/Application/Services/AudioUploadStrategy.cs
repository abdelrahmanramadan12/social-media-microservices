using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Interfaces;

namespace Application.Services
{
    public class AudioUploadStrategy(ICloudinaryCore cloudinaryCore) : IMediaUploadStrategy
    {
        private readonly ICloudinaryCore _cloudinaryCore = cloudinaryCore;

        public async Task<string> UploadAsync(string filePath)
        {
            return await _cloudinaryCore.UploadMediaAsync(filePath, "Audio");
        }
    }
}