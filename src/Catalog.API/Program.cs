using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddKeyedSingleton<IFileProvider>("pics", (services, _) =>
    new PhysicalFileProvider(Path.Combine(
        services.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "Pics")));
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning(options =>
{
    // Include "api-supported-versions" and "api-deprecated-versions" headers in all responses
    options.ReportApiVersions = true;
});

builder.AddDefaultOpenApi(withApiVersioning);

// JWT bearer authentication (enabled when the "Identity" configuration section exists).
builder.AddDefaultAuthentication();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(CatalogAuthorization.ManagerPolicy, policy => policy
        .RequireAuthenticatedUser()
        .RequireClaim("role", CatalogAuthorization.ManagerRole));

// Allow only the configured browser origins (Cors:AllowedOrigins) to call the catalog API.
// Credentials are not allowed: clients authenticate with bearer tokens, not cookies.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStatusCodePages();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapCatalogApi();

app.UseDefaultOpenApi();
app.Run();
