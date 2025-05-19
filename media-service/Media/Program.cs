
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Media
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var connection = builder.Configuration.GetConnectionString("MediaConnection");

            builder.Services.AddDbContext<AppDBContext>(op=>op.UseSqlServer("Data Source=SQL1003.site4now.net;Initial Catalog=db_ab9179_hadzzy;User Id=db_ab9179_hadzzy_admin;Password=Hady01150045098"));


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
