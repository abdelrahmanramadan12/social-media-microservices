using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    public class CloudinaryCore(Cloudinary cloudinary) : ICloudinaryCore
    {
        private readonly Cloudinary _cloudinary = cloudinary;
        private readonly string _noImageUrl = "https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg";

        public Cloudinary GetClient() => _cloudinary;

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls)
        {
            var publicIds = urls.Select(ExtractPublicId).ToList();
            var deletionParams = new DelResParams()
            {
                PublicIds = publicIds,
                Invalidate = true,
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DeleteResourcesAsync(deletionParams);
            
            // Check if all deletions were successful
            if (result.Deleted == null || result.Deleted.Count != publicIds.Count)
            {
                return false;
            }
            
            return true;
        }
        public async Task<bool> DeleteSingleMediaAsync(string id)
        {
                var result = await _cloudinary.DestroyAsync(new DeletionParams(id));
                if (result.Result != "ok")
                    return false;
   
            return true;
        }

        public async Task<RawUploadResult> UploadRawAsync(RawUploadParams uploadParams)
                                    => await _cloudinary.UploadAsync(uploadParams);

        public async Task<string> UploadMediaAsync(string filePath, UsageCategory usageCategory, string? folder = null)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(filePath),
                Folder = folder ?? "uploads",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };

            if (folder == "images")
                uploadParams.Transformation = GetTransformation(usageCategory);


            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl?.ToString() ?? _noImageUrl;
        }

        private static Transformation GetTransformation(UsageCategory imageType)
        {
            return imageType switch
            {
                UsageCategory.Post => new Transformation()
                    .Width(1920).Height(1080).Crop("limit"),

                UsageCategory.Comment => new Transformation()
                    .Width(800).Height(600).Crop("fit"),

                UsageCategory.Message => new Transformation()
                    .Width(600).Height(600).Crop("fill").Gravity("face"),

                UsageCategory.ProfilePicture => new Transformation()
                    .Width(400).Height(400).Crop("thumb").Gravity("face")
                    .Radius("max"), // Circular image

                UsageCategory.CoverPicture => new Transformation()
                    .Width(1500).Height(500).Crop("fill").Gravity("auto"),

                UsageCategory.Story => new Transformation()
                    .Width(1080).Height(1920).Crop("fill").Gravity("auto"),

                _ => throw new ArgumentException("Invalid image type")
            };
        }

        private static string ExtractPublicId(string url)
        {
            // implement logic to extract public ID from URL
            // for example:
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            var publicId = path.Split('/').Last();
            return publicId;
        }

    }
}
