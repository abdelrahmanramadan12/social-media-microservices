using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        Task<string> EditMediaAsync(string MediaUrl, string filePath, string? folder = null);
        Task<string> UploadMediaAsync(string filePath, string? folder = null);
        Task<bool> DeleteMediaAsync(string id);
    }
}
