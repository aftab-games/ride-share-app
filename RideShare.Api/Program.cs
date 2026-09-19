using Microsoft.EntityFrameworkCore;
using RideShare.Api.Hubs;
using RideShare.Infrastructure;
using RideShare.Api.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<RideShareDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RideShareDb")));
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IConnectionTracker, ConnectionTracker>(); //SignalR connection tracker service as a singleton to track connected clients.
builder.Services.AddSingleton<IDriverLocationStore, RedisDriverLocationStore>(); //Driver location store service as a singleton to manage driver locations.
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380")); //Redis connection multiplexer as a singleton to manage Redis connections.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//For testing purposes and avoiding CORS issues, we will use StaticFiles middleware to serve the frontend files from the wwwroot folder. In a production environment, you would typically serve the frontend from a separate web server or CDN.
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<RideHub>("/hubs/ride");

app.Run();