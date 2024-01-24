using Microsoft.EntityFrameworkCore;
using webAPIBrasserie.Models;
using webAPIBrasserie.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<BrasserieDBContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("DBbrasserie")));
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

//app.MapBiereEndpoints();

app.Run();
