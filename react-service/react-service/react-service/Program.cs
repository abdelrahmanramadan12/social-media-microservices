using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Infrastructure;
using react_service.Infrastructure.Mongodb;
using react_service.Application;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.Mapper;
using react_service.Application.Pagination;
using react_service.Application.Services;


var builder = WebApplication.CreateBuilder(args);



builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<PaginationSettings>(
    builder.Configuration.GetSection("Pagination"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<PaginationSettings>>().Value);
builder.Services.AddAutoMapper(typeof(ReactionPostProfile));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<IPostDeletedListener,PostDeletedListener>();
builder.Services.AddScoped<IReactionPublisher, ReactionPublisher>();
builder.Services.AddScoped<react_service.Application.Interfaces.Repositories.IPostRepository, react_service.Infrastructure.Repositories.PostRepository>();



// add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServiceRegistration(builder.Configuration);

// Add controllers and other services
builder.Services.AddControllers();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

//builder.Services.AddHttpClient("GatewayClient", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5000"); // Replace with your gateway URL
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseCors("AllowAll");



app.UseAuthorization();

app.MapControllers();

app.Run();
