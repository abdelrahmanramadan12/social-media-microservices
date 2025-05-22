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
        Task<UploadResponse> UploadMediaAsync(
            MediaUploadRequest request,
            CancellationToken cancellationToken = default);

        Task<UploadResponse> EditMediaAsync(
            MediaUploadRequest newFiles,
            IEnumerable<string> currentUrls,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteMediaAsync(
            IEnumerable<string> urls,
            CancellationToken cancellationToken = default);
    }
}
