using Application;
using Application.Hubs;
using Application.Services.Listeners;
using Infrastructure;
using Application.Hubs;
using Infrastructure.SeedingData.CacheSeeding;
using Infrastructure.SeedingData.mongdbSeeding;
using Infrastructure.Settings.Mongodb;
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

builder.Services.AddSignalR();

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

// ✅ Add RabbitMQ using MassTransit
// Configure settings
//builder.Services.Configure<RabbitMqListenerSettings>("PostListener", builder.Configuration.GetSection("RabbitMQ:PostListener"));
//builder.Services.Configure<RabbitMqListenerSettings>("CommentListener", builder.Configuration.GetSection("RabbitMQ:CommentListener"));

// Register listeners
//builder.Services.AddSingleton<PostListenerService>(sp =>
//{
//    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("PostListener");
//    return new PostListenerService(Options.Create(options));
//});

//builder.Services.AddSingleton<CommentListenerService>(sp =>
//{
//    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("CommentListener");
//    return new CommentListenerService(Options.Create(options));
//});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServiceRegistration(builder.Configuration);

var app = builder.Build();

#region Hub Configuration   
app.MapHub<CommentNotificationHub>("/hubs/comment-notifications");
app.MapHub<FollowNotificationHub>("/hubs/follow-notifications");
app.MapHub<ReactionNotificationHub>("/hubs/reaction-notifications");
app.MapHub<MessageNotificationHub>("/hubs/message-notifications"); 
app.MapHub<ReactionNotificationHub>("/hubs/reactions");
#endregion





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
