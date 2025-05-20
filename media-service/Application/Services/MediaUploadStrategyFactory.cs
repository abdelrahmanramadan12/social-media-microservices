using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MediaUploadStrategyFactory(IServiceProvider provider)
    {
        private readonly IServiceProvider _provider = provider;

        public IMediaUploadStrategy GetStrategy(MediaType type)
        {
            return type switch
            {
                MediaType.Image => _provider.GetRequiredService<ImageUploadStrategy>(),
                MediaType.Video => _provider.GetRequiredService<VideoUploadStrategy>(),
                MediaType.Audio => _provider.GetRequiredService<AudioUploadStrategy>(),
                MediaType.Document => _provider.GetRequiredService<DocumentUploadStrategy>(),
                _ => throw new NotSupportedException($"Media type {type} not supported.")
            };
        }
    }
}
