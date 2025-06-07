using Application.Configuration;
using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure ProfileServiceClient
builder.Services.Configure<ProfileServiceSettings>(
    builder.Configuration.GetSection("Services:Profile"));
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();

// Configure PostServiceClient
builder.Services.Configure<PostServiceSettings>(
    builder.Configuration.GetSection("Services:Post"));
builder.Services.AddHttpClient<IPostServiceClient, PostServiceClient>();

// Configure ReactionServiceClient
builder.Services.Configure<ReactionServiceSettings>(
    builder.Configuration.GetSection("Services:Reaction"));
builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>();

// Configure FollowServiceClient
builder.Services.Configure<FollowServiceSettings>(
    builder.Configuration.GetSection("Services:Follow"));
builder.Services.AddHttpClient<IFollowServiceClient, FollowServiceClient>();

// Register PostAggregationService
builder.Services.AddScoped<IPostAggregationService, PostAggregationService>();

// ... rest of your service configurations

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();