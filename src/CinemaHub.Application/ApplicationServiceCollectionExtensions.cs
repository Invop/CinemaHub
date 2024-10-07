using CinemaHub.Application.Data;
using CinemaHub.Application.Infrastructure.Repositories;
using CinemaHub.Application.Infrastructure.Services;
using CinemaHub.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CinemaHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultAuthentication();

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
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Scoped); // Adjusted to Scoped
        return services;
    }
}
