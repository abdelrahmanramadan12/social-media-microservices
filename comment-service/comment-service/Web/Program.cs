using Domain.IRepository;
using Infrastructure.Repositories;
using MongoDB.Driver;
using Scalar.AspNetCore;
using Service.Implementations.CommentServices;
using Service.Implementations.MediaServices;
using Service.Implementations.PostServices;
using Service.Implementations.RabbitMQServices;
using Service.Interfaces.CommentServices;
using Service.Interfaces.MediaServices;
using Service.Interfaces.PostServices;
using Service.Interfaces.RabbitMQServices;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Add services to the container.
            // Configure MongoDB connection
            var connectionString = builder.Configuration.GetConnectionString("AtlasUri");
            var databaseName = builder.Configuration.GetSection("DatabaseName").Value;

            // Register IMongoClient and IMongoDatabase
            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(connectionString));

            builder.Services.AddSingleton<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));


            // Register repositories
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();

            // Register services
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IPostService, PostService>();

            // Register MediaServiceClient with HttpClient
            builder.Services.AddHttpClient<IMediaServiceClient, MediaServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["MediaService:HostUrl"]);
            });

            // Register RabbitMQ services
            builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
            builder.Services.AddSingleton<ICommentPublisher, CommentPublisher>();

            builder.Services.AddSingleton<IPostListener, PostListener>();

            //builder.Services.AddHostedService<RabbitMqWorker>();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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
