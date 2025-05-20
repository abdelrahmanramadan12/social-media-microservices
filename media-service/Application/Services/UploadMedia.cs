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
        MediaUploadStrategyFactory uploadStrategyFactory): IUploadMediaService
    {
        private readonly MediaUploadStrategyFactory _strategyFactory = uploadStrategyFactory;
        private readonly ICloudinaryCore _cloudinary = cloudinary;
        public async Task<string> UploadAsync(string filePath, MediaType type)
        {
            IMediaUploadStrategy strategy = _strategyFactory.GetStrategy(type);
            var url = await strategy.UploadAsync(filePath);
            return url ?? throw new Exception("Failed to upload media.");
        }

        public async Task<bool> DeleteMediaAsync(string id) => await _cloudinary.DeleteMediaAsync(id);

        public async Task<string> EditMediaAsync(string MediaUrl, string newUrl, string? folder = null)
                                                => await _cloudinary.EditMediaAsync(MediaUrl, newUrl, folder);


    }
}
