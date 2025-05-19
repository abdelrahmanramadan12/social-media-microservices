
using Application.Services;
using Domain.Interfaces;
using DotNetEnv;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Media
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var connection = Environment.GetEnvironmentVariable("MediaConnection");

            builder.Services.AddDbContext<AppDBContext>(op =>
                                                        op.UseSqlServer(connection));

            builder.Services.AddScoped<IUploadMediaService, UploadMediaService>();

            builder.Services.AddScoped<IGetMediaService, GetMediaService>();

            builder.Services.AddScoped<IMediaRepository, MediaRepository>();

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
