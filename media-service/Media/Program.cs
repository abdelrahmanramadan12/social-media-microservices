
using Application.Services;
using CloudinaryDotNet;
using Domain.Interfaces;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Media
{
    public class Program
    {
        // Update your Program.cs
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Configure Cloudinary
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            // Register Cloudinary instance
            builder.Services.AddSingleton(provider => {
                var config = provider.GetRequiredService<IOptions<CloudinarySettings>>();
                var account = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
                return new Cloudinary(account);
            });

            // Register your services
            builder.Services.AddScoped<ICloudinaryCore, CloudinaryCore>();
            builder.Services.AddScoped<ImageUploadStrategy>();
            builder.Services.AddScoped<VideoUploadStrategy>();
            builder.Services.AddScoped<AudioUploadStrategy>();
            builder.Services.AddScoped<DocumentUploadStrategy>();
            builder.Services.AddScoped<MediaUploadStrategyFactory>();
            builder.Services.AddScoped<IUploadMediaService, UploadMediaService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
