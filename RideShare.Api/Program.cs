using Microsoft.EntityFrameworkCore;
using RideShare.Api.Hubs;
using RideShare.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RideShareDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RideShareDb")));

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<RideHub>("/hubs/ride");

app.Run();