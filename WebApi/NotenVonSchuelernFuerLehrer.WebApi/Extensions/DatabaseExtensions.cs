using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
        await migrationService.MigrateAsync();
    }

    public static async Task SeedTestData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var testdatenAnlegenService = scope.ServiceProvider.GetRequiredService<TestdatenAnlegenService>();
        await testdatenAnlegenService.SeedAsync();
    }
}