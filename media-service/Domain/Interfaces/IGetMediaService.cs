using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGetMediaService
    {
        Task<string> GetMediaURL(Guid mediaId);
        Task<Media?> GetMediaAsync(string mediaUrl);
        Task<Media?> EditMediaAsync(string MediaUrl);
    }
}
