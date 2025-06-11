using Application.Services.Implementations;
using Application.Services.Interfaces;
using Application.Configurations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Bind configuration settings
builder.Services.Configure<ReactionServiceSettings>(
    builder.Configuration.GetSection(ReactionServiceSettings.ConfigurationSection));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<ReactionServiceSettings>>().Value);

builder.Services.Configure<ProfileServiceSettings>(
    builder.Configuration.GetSection(ProfileServiceSettings.ConfigurationSection));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<ProfileServiceSettings>>().Value);

// Configure HttpClients
builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>();
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();

// Register aggregation service
builder.Services.AddScoped<IReactionsAggregationService, ReactionsAggregationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
