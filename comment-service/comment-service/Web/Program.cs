using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Scalar.AspNetCore;
using Service.Implementations;
using Service.Interfaces;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Configure MongoDB connection
            var connectionString = builder.Configuration.GetConnectionString("AtlasUri");
            var databaseName = builder.Configuration.GetSection("DatabaseName").Value;

            // Register IMongoClient and IMongoDatabase
            builder.Services.AddSingleton<IMongoClient>(sp => 
                new MongoClient(connectionString));
            
            builder.Services.AddSingleton<IMongoDatabase>(sp => 
                sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            //// EntityFrameworkCore configuration (if you still need it)
            //builder.Services.AddDbContext<CommentContext>(options =>
            //{
            //    options.UseMongoDB(connectionString, databaseName);
            //});

            // Register repositories
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            
            // Register services
            builder.Services.AddScoped<ICommentService, CommentService>();

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
