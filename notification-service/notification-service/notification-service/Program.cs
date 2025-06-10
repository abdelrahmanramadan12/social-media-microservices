using Application;
using Application.Hubs;
using Application.Interfaces.Listeners;
using Application.Services.Listeners;
using Domain.RabbitMQ;
using Infrastructure;
using Infrastructure.SeedingData.CacheSeeding;
using Infrastructure.SeedingData.mongdbSeeding;
using Infrastructure.Settings.Mongodb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using Workers;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Configuration
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Redis Configuration
builder.Services.AddScoped<IConnectionMultiplexer>(sp =>
{
    var redisSettings = builder.Configuration.GetSection("RedisSettings");
    var host = redisSettings["Host"];
    var port = int.Parse(redisSettings["Port"]);
    var user = redisSettings["User"];
    var password = redisSettings["Password"];

    return ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { { host, port } },
        User = user,
        Password = password
    });
});

// SignalR
builder.Services.AddSignalR();

// RabbitMQ Settings Configuration
builder.Services.Configure<RabbitMqListenerSettings>("FollowListener", builder.Configuration.GetSection("RabbitMQ:FollowListener"));
builder.Services.Configure<RabbitMqListenerSettings>("CommentListener", builder.Configuration.GetSection("RabbitMQ:CommentListener"));
builder.Services.Configure<RabbitMqListenerSettings>("ReactionListener", builder.Configuration.GetSection("RabbitMQ:ReactionListener"));
builder.Services.Configure<RabbitMqListenerSettings>("MessageListener", builder.Configuration.GetSection("RabbitMQ:MessageListener"));

// Register RabbitMQ Listener Services
builder.Services.AddSingleton<CommentListenerService>(sp =>
{
    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("CommentListener");
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new CommentListenerService(Options.Create(options), scopeFactory);
});
builder.Services.AddSingleton<ICommentListener>(sp => sp.GetRequiredService<CommentListenerService>());

builder.Services.AddSingleton<ReactionListenerService>(sp =>
{
    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("ReactionListener");
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new ReactionListenerService(Options.Create(options), scopeFactory);
});
builder.Services.AddSingleton<IReactionListener>(sp => sp.GetRequiredService<ReactionListenerService>());

builder.Services.AddSingleton<MessageListenerService>(sp =>
{
    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("MessageListener");
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new MessageListenerService(Options.Create(options), scopeFactory);
});
builder.Services.AddSingleton<IMessageListener>(sp => sp.GetRequiredService<MessageListenerService>());

builder.Services.AddSingleton<FollowListenerService>(sp =>
{
    var options = sp.GetRequiredService<IOptionsMonitor<RabbitMqListenerSettings>>().Get("FollowListener");
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new FollowListenerService(Options.Create(options), scopeFactory);
});
builder.Services.AddSingleton<IFollowListener>(sp => sp.GetRequiredService<FollowListenerService>());

// Hosted RabbitMQ Worker
builder.Services.AddHostedService<RabbitMqWorker>();

// Register Application & Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServiceRegistration(builder.Configuration);

var app = builder.Build();

// Hubs
app.MapHub<CommentNotificationHub>("/hubs/comment-notifications");
app.MapHub<FollowNotificationHub>("/hubs/follow-notifications");
app.MapHub<ReactionNotificationHub>("/hubs/reaction-notifications");
app.MapHub<MessageNotificationHub>("/hubs/message-notifications");
app.MapHub<ReactionNotificationHub>("/hubs/reactions");

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
