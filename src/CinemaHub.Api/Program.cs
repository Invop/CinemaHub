using CinemaHub.Api.Mapping;
using CinemaHub.Application;
using CinemaHub.Application.Data;
using CinemaHub.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.AddServiceDefaults();
builder.AddApplicationServices();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
builder.AddNpgsqlDbContext<MovieDbContext>("moviesdb");
builder.Services.AddMigration<MovieDbContext>();
builder.Services.AddControllers();
builder.Services.AddApplication();


var app = builder.Build();
app.MapDefaultEndpoints();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

app.UseDefaultOpenApi();
app.Run();
