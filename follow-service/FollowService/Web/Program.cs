using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;
using Scalar.AspNetCore;
using Services.Implementations;
using Services.Interfaces;
using Workers;

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

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFollowRepository, FollowRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IFollowCommandService, FollowCommandService>();
            builder.Services.AddScoped<IFollowQueryService, FollowQueryService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddSingleton<IProfileCreatedListener, ProfileCreatedListener>();
            builder.Services.AddSingleton<IProfileDeletedListener, ProfileDeletedListener>();
            
            //builder.Services.AddHostedService<RabbitMqWorker>();

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
