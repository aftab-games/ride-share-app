using Microsoft.EntityFrameworkCore;
using RideShare.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RideShareDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RideShareDb")));

builder.Services.AddControllers();

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

app.Run();