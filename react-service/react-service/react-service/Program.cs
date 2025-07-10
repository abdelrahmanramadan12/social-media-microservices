using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Application;
using react_service.Application.Events;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Pagination;
using react_service.Infrastructure;
using react_service.Infrastructure.Mongodb;
using react_service.Infrastructure.Repositories;
using Worker;
using Workers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(options =>
{
    builder.Configuration.GetSection("MongoDbSettings").Bind(options);
    var dbSection = builder.Configuration.GetSection("Databases");
    options.PostsDatabaseName = dbSection["PostsDatabaseName"];
    options.ReactionsDatabaseName = dbSection["ReactionsDatabaseName"];
});
builder.Services.Configure<PaginationSettings>(
    builder.Configuration.GetSection("Pagination"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<PaginationSettings>>().Value);

// Register MongoDB clients for different databases
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register database access for Posts
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.PostsDatabaseName ?? throw new InvalidOperationException("Posts database name is not configured"));
});

// Register database access for Reactions
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.ReactionsDatabaseName ?? throw new InvalidOperationException("Reactions database name is not configured"));
});

// Register RabbitMQ services
builder.Services.AddSingleton<IPostEventListner, PostEventListener>();
builder.Services.AddSingleton<ICommentEventListner, CommentEventListener>();
builder.Services.AddSingleton<IQueuePublisher<ReactionEvent>, ReactionEventPublisher>();
builder.Services.AddSingleton<IQueuePublisher<CommentReactionEvent>, CommentReactionPublisher>();
builder.Services.AddSingleton<IQueuePublisher<ReactionEventNoti>, ReactionEventNotiPublisher>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();

// Register QueuePublishInitializer as a background service
builder.Services.AddHostedService<QueuePublishersInitializer>();
builder.Services.AddHostedService<QueueListenerWorker>();

// add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServiceRegistration(builder.Configuration);

// Add controllers and other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
