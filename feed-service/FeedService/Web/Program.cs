using MongoDB.Driver;
using Web.ServiceCollections;
using Workers;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(builder.Configuration.GetConnectionString("AtlasUri")));
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration.GetSection("DatabaseName").Value));

            builder.Services.AddFeedServices();
            builder.Services.AddServiceClients();
            builder.Services.AddQueueListeners();

            builder.Services.AddHostedService<RabbitMqWorker>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
