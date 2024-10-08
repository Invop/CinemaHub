using MovieHub.Api;
using MovieHub.Api.Mapping;
using MovieHub.Application;
using MovieHub.Application.Data;
using MovieHub.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.AddServiceDefaults();
builder.AddApplicationServices();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
builder.AddNpgsqlDbContext<MovieDbContext>("moviesdb");
builder.Services.AddDatabase();
builder.Services.AddControllers();
builder.Services.AddApplication();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

app.UseDefaultOpenApi();
app.Run();