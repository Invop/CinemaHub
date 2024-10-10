using MovieHub.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieHub.Application.Data;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Infrastructure.Services;

namespace MovieHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultAuthentication();
        builder.AddRedisOutputCache("redis");
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddMigration<MovieDbContext,MoviesSeeder>();
        return services;
    }
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
        return services;
    }
}
