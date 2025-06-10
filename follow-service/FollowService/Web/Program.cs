using Infrastructure.Repositories;
using MongoDB.Driver;
using Scalar.AspNetCore;
using Application.Implementations;
using Application.Abstractions;
using Workers.Listeners;
using Workers;
using Application.Events;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(builder.Configuration.GetConnectionString("AtlasUri")));
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration.GetSection("DatabaseName").Value));

            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IFollowRepository, FollowRepository>();
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IFollowCommandService, FollowCommandService>();
            builder.Services.AddScoped<IFollowQueryService, FollowQueryService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            builder.Services.AddSingleton<IQueueListener<ProfileEvent>, ProfileQueueListener>();
            builder.Services.AddSingleton<IQueuePublisher<FollowEvent>, FollowQueuePublisher>();

            builder.Services.AddHostedService<QueueListenersWorker>();
            builder.Services.AddHostedService<QueuePublishersInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
