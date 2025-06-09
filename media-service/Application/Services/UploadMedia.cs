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

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls)
            => await _cloudinary.DeleteMediaAsync(urls);

        private async Task<bool> TryAlternativeDeleteMethods(string url)
        {
            try
            {
                var uri = new Uri(url);
                var filename = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                var result = await _cloudinary.DeleteSingleMediaAsync(filename);
                if (result) return true;
            }
            catch { }
            
            try
            {
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                var lastSlashPos = path.LastIndexOf('/');
                var secondLastSlashPos = path.LastIndexOf('/', lastSlashPos - 1);
                
                if (lastSlashPos > 0 && secondLastSlashPos > 0)
                {
                    var folderAndFile = path.Substring(secondLastSlashPos + 1);
                    
                    var lastDot = folderAndFile.LastIndexOf('.');
                    var publicId = lastDot > 0 ? folderAndFile.Substring(0, lastDot) : folderAndFile;
                    
                    var result = await _cloudinary.DeleteSingleMediaAsync(publicId);
                    if (result) return true;
                }
            }
            catch { }
            
            try
            {
                var result = await _cloudinary.DeleteSingleMediaAsync(url);
                if (result) return true;
            }
            catch { }
            
            return false;
        }

        
        private string ExtractPublicId(string urlOrId)
        {
            if (!urlOrId.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return urlOrId;
            try
            {
                var uri = new Uri(urlOrId);
                var path = uri.AbsolutePath;
                
                int uploadPos = path.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
                if (uploadPos < 0)
                    return urlOrId; 
                
                int versionStart = uploadPos + "/upload/".Length;
                int folderStart = path.IndexOf('/', versionStart);
                
                if (folderStart < 0)
                    return urlOrId;
                
                string publicIdWithExt = path.Substring(folderStart + 1);
                
                int lastDot = publicIdWithExt.LastIndexOf('.');
                return lastDot > 0 ? publicIdWithExt.Substring(0, lastDot) : publicIdWithExt;
            }
            catch
            {
                return urlOrId;
            }
        }
    }
}
