using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotenVonSchuelernFuerLehrerDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotenVonSchuelernFuerLehrerDbContext>>();
 
        const int maxRetryAttempts = 30;
        const int maxRetryDelayInSeconds = 2;
        
        var attempt = 0;
        while (true)
        {
            try
            {
                attempt++;
                logger.LogInformation("Versuche DB-Migration (Versuch {Attempt}/{Max})", attempt, maxRetryAttempts);
                await context.Database.MigrateAsync();
                logger.LogInformation("DB-Migration erfolgreich");
                return;
            }
            catch (MySqlException ex)
            {
                if (attempt >= maxRetryAttempts)
                {
                    logger.LogError(ex, "DB-Migration fehlgeschlagen nach {Max} Versuchen", maxRetryAttempts);
                    throw;
                }
                
                await Task.Delay(TimeSpan.FromSeconds(maxRetryDelayInSeconds));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unerwarteter Fehler bei DB-Migration");
                throw;
            }
        }
    }
}