using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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


// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Your API Name",
        Version = "v1",
        Description = "API documentation for Your API",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your@email.com",
            Url = new Uri("https://yourwebsite.com")
        }
    });
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");

// Enable middleware to serve generated Swagger as a JSON endpoint
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at app root (localhost:5000/)
});

app.UseAuthorization();

app.MapControllers();

app.Run();
