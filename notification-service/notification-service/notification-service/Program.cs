using Application;
using Infrastructure;
using Infrastructure.Data.Seeders;
using Infrastructure.Mongodb;
using Infrastructure.Redis;
using Infrastructure.SeedingData.CacheSeeding;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDbSettings"));
// 1. Register IMongoClient as singleton (thread-safe)
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// 2. Register IMongoDatabase as scoped
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("RedisSettings:ConnectionString"))
);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServiceRegistration(builder.Configuration);
builder.Services.AddScoped<RedisFollowsSeeder>();   
builder.Services.AddScoped<MongoFollowsSeeder>();   
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var cacheSeeder = scope.ServiceProvider.GetRequiredService<RedisFollowsSeeder>();
            
            await cacheSeeder.SeedInitialFollowsDataAsync();
            var mongoSeeder = scope.ServiceProvider.GetRequiredService<MongoFollowsSeeder>();
            await mongoSeeder.SeedInitialFollowsDataAsync();


        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding Redis data");
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
