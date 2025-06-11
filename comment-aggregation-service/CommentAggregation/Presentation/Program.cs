using Application.Services.Implementation;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure named HttpClients from configuration
builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Clients:Reaction"]);
});

builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Clients:Profile"]);
});

builder.Services.AddHttpClient<ICommentServiceClient, CommentServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Clients:Comment"]);
});

// Scoped service
builder.Services.AddScoped<ICommentAggregationService, CommentAggregationService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
