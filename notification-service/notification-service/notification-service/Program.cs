using Application;
using Infrastructure;
using Infrastructure.Mongodb;
using Infrastructure.Redis;
using Infrastructure.SeedingData.CacheSeeding;
using Infrastructure.SeedingData.mongdbSeeding;
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
            // seed CacheDB data 
            var followsCacheSeeder = scope.ServiceProvider.GetRequiredService<RedisFollowsSeeder>();
            
            await followsCacheSeeder.SeedInitialFollowsDataAsync();

            var reactionCacheReactions = scope.ServiceProvider.GetRequiredService<RedisReactionsSeeder>();
            await reactionCacheReactions.SeedInitialReactionsDataAsync();

            var commentsCacheSeeder =  scope.ServiceProvider.GetRequiredService<RedisCommentsSeeder>();

            await commentsCacheSeeder.SeedInitialCommentsDataAsync();
            // Seed MongoDB data
            var mongoFollowsSeeder = scope.ServiceProvider.GetRequiredService<MongoFollowsSeeder>();
            await mongoFollowsSeeder.SeedInitialFollowsDataAsync();
            
            var mongoReactionsSeeder = scope.ServiceProvider.GetRequiredService<MongoReactionsSeeder>();
            await mongoReactionsSeeder.SeedInitialReactionsDataAsync();
            
            var mongoCommentsSeeder = scope.ServiceProvider.GetRequiredService<MongoCommentsSeeder>();
            await mongoCommentsSeeder.SeedInitialCommentsDataAsync();




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
