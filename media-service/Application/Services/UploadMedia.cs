using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UploadMediaService(
        ICloudinaryCore cloudinary,
        MediaUploadStrategyFactory uploadStrategyFactory) : IUploadMediaService
    {
        private readonly MediaUploadStrategyFactory _strategyFactory = uploadStrategyFactory;
        private readonly ICloudinaryCore _cloudinary = cloudinary;
        public async Task<string> UploadAsync(string filePath, MediaType type, UsageCategory usageCategory)
        {
            IMediaUploadStrategy strategy = _strategyFactory.GetStrategy(type, usageCategory);
            var url = await strategy.UploadAsync(filePath, usageCategory);
            return url ?? throw new Exception("Failed to upload media.");
        }

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> id) => await _cloudinary.DeleteMediaAsync(id);

        public async Task<string> EditMediaAsync(string MediaUrl, string newUrl, UsageCategory usageCategory, string? folder = null)
                                                => await _cloudinary.EditMediaAsync(MediaUrl, newUrl, usageCategory, folder);


    }
}
