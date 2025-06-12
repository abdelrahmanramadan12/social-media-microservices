var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()     // Allow all domains
            .AllowAnyMethod()     // Allow GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader();    // Allow any headers
    });
});

// Add YARP services
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<UserIdHeaderMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll"); // Apply CORS before routing

app.MapReverseProxy();

app.MapControllers();

app.Run();
