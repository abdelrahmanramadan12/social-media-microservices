using Application.Services.Implementation;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


// Register HttpClient and ReactionServiceClient
builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>();
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();
builder.Services.AddHttpClient<ICommentServiceClient, CommentServiceClient>();
builder.Services.AddScoped<ICommentAggregationService, CommentAggregationService>();


var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
