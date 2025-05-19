using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUploadMediaService
    {
        public Task<string> UploadMediaAsync(IFormFile file, string? description);

        public Task<bool> DeleteMediaAsync(Guid id);

        public Task<bool> EditMediaAsync(string MediaUrl);
    }
}
