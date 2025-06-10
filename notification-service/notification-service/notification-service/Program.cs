using Application;
using Application.Hubs;
using Application.Interfaces.Listeners;
using Application.Interfaces.Services.Application.Services;
using Application.Interfaces.Services;
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

var profileServiceUrl = builder.Configuration["ProfileService:BaseUrl"];

builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>(client =>
{
    client.BaseAddress = new Uri(profileServiceUrl);
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

// add SignalR 

builder.Services.AddSignalR();

builder.Services.AddHostedService<RabbitMqWorker>();

// Register Application & Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRabbitMqListeners(builder.Configuration);
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
