using Application.Configuration;
using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Post service
var postServiceSettings = builder.Configuration.GetSection(PostServiceSettings.ConfigurationSection).Get<PostServiceSettings>();
builder.Services.AddSingleton(postServiceSettings);
builder.Services.AddHttpClient<IpostServiceClient, PostServiceClient>();

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
