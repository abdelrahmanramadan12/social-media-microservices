using Application.Abstractions;
using Application.Services;
using Infrastructure.Caches;
using Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;
using StackExchange.Redis;
using Web.Hubs;

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

            builder.Services.AddSignalR();

            // mongodb injection
            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(builder.Configuration.GetConnectionString("AtlasUri")));
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration.GetSection("DatabaseName").Value));

            // redis injection
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisAddress")!));
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

            // services injection
            builder.Services.AddSingleton<IMessageRepository, MessagesRepository>();
            builder.Services.AddSingleton<IConversationRepository, ConversationsRepository>();
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
            builder.Services.AddSingleton<IProfileCache, ProfileCache>();

            var config = builder.Configuration;

            builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(config["HttpClients:Auth"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(config["HttpClients:Profile"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddHttpClient<IMediaServiceClient, MediaServiceClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(config["HttpClients:Media"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddScoped<IRealtimeMessenger, RealtimeMessenger>();
            builder.Services.AddScoped<IChatCommandService,  ChatCommandService>();
            builder.Services.AddScoped<IChatQueryService,  ChatQueryService>();

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
