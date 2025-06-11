using Application.Configuration;
using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//// Configure Post service
//var postServiceSettings = builder.Configuration.GetSection(PostServiceSettings.ConfigurationSection).Get<PostServiceSettings>();
//builder.Services.AddSingleton(postServiceSettings);
//builder.Services.AddHttpClient<IPostServiceClient, PostServiceClient>();


//// Configure Reaction service
//var reactionServiceSettings = builder.Configuration.GetSection(ReactionServiceSettings.ConfigurationSection).Get<ReactionServiceSettings>();
//builder.Services.AddSingleton(reactionServiceSettings);
//builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>();

//// Configure Follow service
//var followServiceSettings = builder.Configuration.GetSection(FollowServiceSettings.ConfigurationSection).Get<FollowServiceSettings>();
//builder.Services.AddSingleton(followServiceSettings);
//builder.Services.AddHttpClient<IFollowServiceClient, FollowServiceClient>();

//// Configure Profile service

//var profileServiceSettings = builder.Configuration.GetSection(ProfileServiceSettings.ConfigurationSection).Get<ProfileServiceSettings>();
//builder.Services.AddSingleton(profileServiceSettings);
//builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();


builder.Services.AddHttpClient<IPostServiceClient, PostServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Services:Post"]);
});

builder.Services.AddHttpClient<IReactionServiceClient, ReactionServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Services:Reaction"]);
});

builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Services:Profile"]);
});

builder.Services.AddHttpClient<IFollowServiceClient, FollowServiceClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Services:Follow"]);
});


builder.Services.AddScoped<IPostAggregationService, PostAggregationService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
