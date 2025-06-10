using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure ProfileServiceClient
builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("Services:Profile").Value ?? throw new NullReferenceException("Missing profile service Base Uri"));
});

// Configure FollowServiceClient
builder.Services.AddHttpClient<IFollowServiceClient, FollowServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("Services:Follow").Value ?? throw new NullReferenceException("Missing follow service Base Uri"));
});

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
