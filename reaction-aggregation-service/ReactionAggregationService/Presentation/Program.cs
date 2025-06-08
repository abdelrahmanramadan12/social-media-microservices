using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure ReactionServiceClient
builder.Services.Configure<ReactionServiceClient>(
    builder.Configuration.GetSection("Services:Reaction"));
builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>();

// Configure ProfileServiceClient
builder.Services.Configure<ReactionServiceClient>(
    builder.Configuration.GetSection("Services:Reaction"));
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();

// Register PostAggregationService
builder.Services.AddScoped<IReactionsAggregationService, ReactionsAggregationService>();

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
