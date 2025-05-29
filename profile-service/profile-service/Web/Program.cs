
using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Service.Implementations.FollowServices;
using Service.Implementations.ProfileServices;
using Service.Implementations.RabbitMqServices;
using Service.Implementations.RabbitMQServices;
using Service.Interfaces.FollowServices;
using Service.Interfaces.ProfileServices;
using Service.Interfaces.RabbitMqServices;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            var connectionString = builder.Configuration.GetConnectionString("TestConnection");

            builder.Services.AddDbContext<ProfileDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IFollowCounterService, FollowCounterService>();

            builder.Services.AddScoped<IFollowListener, FollowListener>();

            builder.Services.AddScoped<IProfilePublisher, ProfilePublisher>();
            builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

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
