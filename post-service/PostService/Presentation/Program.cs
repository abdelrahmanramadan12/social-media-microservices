using Application.Configuration;
using Application.DTOs;
using Application.Events;
using Application.Implementations;
using Application.Interfaces;
using Application.IServices;
using Application.Services;
using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;
using Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.Configure<MediaServiceSettings>(builder.Configuration.GetSection("MediaService"));
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("Encryption"));

// Configure MongoDB
builder.Services.AddSingleton<IPostRepository>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new PostRepository(settings.ConnectionString, settings.DatabaseName);
});

// Configure Media Service Client
builder.Services.AddHttpClient<IMediaServiceClient, MediaServiceClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<MediaServiceSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});

// Configure Application Services
builder.Services.AddScoped<IEncryptionService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<EncryptionSettings>>().Value;
    return new EncryptionService(settings.Key);
});

builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<IPostService, PostService>();

// Configure RabbitMQ Publisher
builder.Services.AddSingleton<IQueuePublisher<PostEvent>, PostEntityQueuePublisher>();

builder.Services.AddHostedService<QueueListenersWorker>();
builder.Services.AddHostedService<QueuePublishersInitializer>();
builder.Services.AddHostedService<CommentEventConsumer>();
builder.Services.AddHostedService<ReactionEventConsumer>();

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
