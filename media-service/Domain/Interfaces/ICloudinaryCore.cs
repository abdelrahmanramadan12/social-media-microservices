using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICloudinaryCore
    {
        //Cloudinary GetClient();
        Task<RawUploadResult> UploadRawAsync(RawUploadParams uploadParams);

        Task<string> UploadMediaAsync(string filePath, UsageCategory usageCategory, string? folder = null);
        Task<bool> DeleteMediaAsync(IEnumerable<string> urls);
        Task<bool> DeleteSingleMediaAsync(string id);
    }
}
