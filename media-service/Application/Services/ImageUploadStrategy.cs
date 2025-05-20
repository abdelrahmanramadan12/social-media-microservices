using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ImageUploadStrategy(ICloudinaryCore cloudinaryCore) : IMediaUploadStrategy
    {
        private readonly ICloudinaryCore _cloudinaryCore = cloudinaryCore;

        public async Task<string> UploadAsync(string filePath)
        {
            return await _cloudinaryCore.UploadMediaAsync(filePath, "images");
        }
    }

}

