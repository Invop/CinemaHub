using CinemaHub.ServiceDefaults;
using Duende.IdentityServer.AspNetIdentity;
using Identity.Api;
using Identity.Api.Configuration;
using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews();

builder.AddNpgsqlDbContext<ApplicationDbContext>("identitydb");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
    {
        //options.IssuerUri = "null";
        options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

        // TODO: Remove this line in production.
        options.KeyManagement.Enabled = false;
    })
    .AddInMemoryIdentityResources(Config.GetResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddInMemoryApiResources(Config.GetApis())
    .AddInMemoryClients(Config.GetClients(builder.Configuration))
    .AddAspNetIdentity<ApplicationUser>()
// TODO: Not recommended for production - you need to store your key material somewhere secure
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();

// This cookie policy fixes login issues with Chrome 80+ using HTTP
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();