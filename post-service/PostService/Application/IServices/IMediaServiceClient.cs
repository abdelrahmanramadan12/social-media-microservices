using Application.DTOs;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IMediaServiceClient
    {
        Task<MediaUploadResponse> UploadMediaAsync(
            MediaUploadRequest request,
            CancellationToken cancellationToken = default);

        Task<MediaUploadResponse> EditMediaAsync(
            MediaUploadRequest newFiles,
            IEnumerable<string> currentUrls,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteMediaAsync(
            IEnumerable<string> urls,
            CancellationToken cancellationToken = default);
    }
}
