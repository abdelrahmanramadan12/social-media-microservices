using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services
{
    public class AudioUploadStrategy(ICloudinaryCore cloudinaryCore) : IMediaUploadStrategy
    {
        private readonly ICloudinaryCore _cloudinaryCore = cloudinaryCore;

        public async Task<string> UploadAsync(string filePath, UsageCategory usageCategory)
        {
            return await _cloudinaryCore.UploadMediaAsync(filePath, usageCategory, "Audio");
        }
    }
}