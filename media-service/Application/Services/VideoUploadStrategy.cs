using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VideoUploadStrategy(ICloudinaryCore cloudinary) : IMediaUploadStrategy
    {
        private readonly ICloudinaryCore _cloudinaryCore = cloudinary;

        public async Task<string> UploadAsync(string filePath, UsageCategory usageCategory)
        {
            return await _cloudinaryCore.UploadMediaAsync(filePath, usageCategory, "videos");
        }
    }
}
