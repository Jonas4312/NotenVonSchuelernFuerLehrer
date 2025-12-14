using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>("ShouldMigrateDatabase"))
        {
            using var scope = app.Services.CreateScope();
            var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
            await migrationService.MigrateAsync();
        }
    }

    public static async Task SeedTestData(this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>("ShouldSeedDatabase"))
        {
            using var scope = app.Services.CreateScope();
            var testdatenAnlegenService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
            await testdatenAnlegenService.SeedAsync();
        }
    }

    public static void UseConfiguredExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
        
                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Success = false,
                    Errors = ["Ein unerwarteter Fehler ist aufgetreten. Bitte versuchen Sie es später erneut."]
                });
            });
        });
    }
}