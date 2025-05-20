using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUploadMediaService
    {

        Task<string> UploadAsync(string filePath, MediaType type);

        Task<bool> DeleteMediaAsync(string id);

        Task<string> EditMediaAsync(string MediaUrl, string newUrl, string? folder = null);

    }
}
