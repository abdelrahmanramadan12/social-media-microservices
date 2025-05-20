using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Interfaces;

namespace Application.Services
{
    public class DocumentUploadStrategy(ICloudinaryCore cloudinaryCore) : IMediaUploadStrategy
    {
        private readonly ICloudinaryCore _cloudinaryCore = cloudinaryCore;

        public async Task<string> UploadAsync(string filePath)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(filePath),
                Folder = "documents",
                UseFilename = true,
                UniqueFilename = true,
                PublicId = Path.GetFileNameWithoutExtension(filePath)
            };

            var result = await _cloudinaryCore.UploadRawAsync(uploadParams);
            return result.SecureUrl?.ToString();
        }
    }
}