using MovieHub.Api.Mapping;
using MovieHub.Application;
using MovieHub.Application.Data;
using MovieHub.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

builder.AddNpgsqlDbContext<MovieDbContext>("moviesdb");
builder.Services.AddDatabase();


builder.Services.AddApplication();
builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c => 
        c.Cache()
            .Expire(TimeSpan.FromMinutes(1))
            .SetVaryByQuery(["title", "year", "sortBy", "page", "pageSize"])
            .Tag("movies"));
    
    
});

builder.Services.AddControllers();


var app = builder.Build();

app.MapDefaultEndpoints();
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

app.UseDefaultOpenApi();

app.Run();