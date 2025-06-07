using Application.Configuration;
using Application.Services.Implementations;
using Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure ProfileServiceClient
builder.Services.Configure<ProfileServiceSettings>(
    builder.Configuration.GetSection("ProfileService"));

builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();

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