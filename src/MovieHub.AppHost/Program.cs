using MovieHub.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddForwardedHeaders();

// Configure the main postgres container
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

// Create individual database references
var identityDb = postgres.AddDatabase("identitydb");
var moviesDb = postgres.AddDatabase("moviesdb");

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

// Services
var identityApi = builder.AddProject<Projects.Identity_Api>("identity-api", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithReference(identityDb);

var identityEndpoint = identityApi.GetEndpoint(launchProfileName);

var moviesApi = builder.AddProject<Projects.MovieHub_Api>("movies-api")
    .WithReference(moviesDb)
    .WithEnvironment("Identity__Url", identityEndpoint);

// Apps
var webApp = builder.AddProject<Projects.WebApp>("webapp", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithEnvironment("IdentityUrl", identityEndpoint);

// Wire up the callback urls (self referencing)
webApp.WithEnvironment("CallBackUrl", webApp.GetEndpoint(launchProfileName));

identityApi
    .WithEnvironment("MoviesApiClient", moviesApi.GetEndpoint(launchProfileName))
    .WithEnvironment("WebAppClient", webApp.GetEndpoint(launchProfileName));

builder.Build().Run();

// For test use only.
// Looks for an environment variable that forces the use of HTTP for all the endpoints. We
// are doing this for ease of running the Playwright tests in CI.
static bool ShouldUseHttpForEndpoints()
{
    const string envVarName = "CinemaHub_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(envVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}
