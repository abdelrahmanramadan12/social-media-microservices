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

        Task<string> UploadAsync(string filePath, MediaType type ,UsageCategory usageCategory);

        Task<bool> DeleteMediaAsync(IEnumerable<string> urls);

    }
}
