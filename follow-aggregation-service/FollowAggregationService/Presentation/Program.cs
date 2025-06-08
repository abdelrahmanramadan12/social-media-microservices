using Application.Configurations;
using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure ProfileServiceClient
builder.Services.Configure<ProfileServiceSettings>(
    builder.Configuration.GetSection("Services:Profile"));
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();

// Configure FollowServiceClient
builder.Services.Configure<FollowServiceSettings>(
    builder.Configuration.GetSection("Services:Follow"));
builder.Services.AddHttpClient<IFollowServiceClient, FollowServiceClient>();

// Register PostAggregationService
builder.Services.AddScoped<IFollowAggregationService, FollowAggregationService>();


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
