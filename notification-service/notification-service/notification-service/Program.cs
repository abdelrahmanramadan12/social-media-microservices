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

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddScoped<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { { builder.Configuration.GetConnectionString("RedisConnection")!,
                            int.Parse(builder.Configuration.GetConnectionString("RedisPort")!) } },
        User = builder.Configuration.GetConnectionString("RedisUserName"),
        Password = builder.Configuration.GetConnectionString("RedisPassword")
    });
});



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

            var commentsCacheSeeder = scope.ServiceProvider.GetRequiredService<RedisCommentsSeeder>();

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
