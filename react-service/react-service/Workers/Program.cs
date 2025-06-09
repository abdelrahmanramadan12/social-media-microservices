using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Repositories;
using react_service.Infrastructure.Repositories;
using System;

namespace Workers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting application: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    try
                    {
                        // Register repositories
                        services.AddSingleton<IPostRepository, PostRepository>();
                        
                        // Register listeners
                        services.AddSingleton<IPostEventListner, PostEventListner>();
                        
                        // Register consumers
                        services.AddSingleton<PostEventListner>();
                        
                        // Register publishers
                        services.AddSingleton<PostEventPublisher>();
                        services.AddSingleton<ReactionEventPublisher>();
                        
                        // Register background service
                        services.AddHostedService<RabbitMqWorker>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring services: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        throw;
                    }
                });
    }
} 