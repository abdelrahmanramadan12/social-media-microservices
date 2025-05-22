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

        public IMediaUploadStrategy GetStrategy(MediaType type, UsageCategory usageCategory)
        {
            return type switch
            {
                MediaType.IMAGE => _provider.GetRequiredService<ImageUploadStrategy>(),
                MediaType.VIDEO => _provider.GetRequiredService<VideoUploadStrategy>(),
                MediaType.AUDIO => _provider.GetRequiredService<AudioUploadStrategy>(),
                MediaType.DOCUMENT => _provider.GetRequiredService<DocumentUploadStrategy>(),
                _ => throw new NotSupportedException($"Media type {type} not supported. or Usage Category is unavailable{usageCategory}")
            };
        }
    }
}
