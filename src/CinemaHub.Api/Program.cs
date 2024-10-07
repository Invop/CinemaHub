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

builder.Services.AddControllers();
builder.Services.AddApplication();

builder.AddNpgsqlDbContext<MovieDbContext>("moviesdb");
builder.Services.AddMigration<MovieDbContext>();
var app = builder.Build();

app.UseDefaultOpenApi();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();
app.Run();
