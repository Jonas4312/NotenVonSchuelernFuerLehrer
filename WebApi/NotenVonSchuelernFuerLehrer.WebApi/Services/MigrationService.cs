using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class MigrationService
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly ILogger<NotenVonSchuelernFuerLehrerDbContext> _logger;

    public MigrationService(NotenVonSchuelernFuerLehrerDbContext context, ILogger<NotenVonSchuelernFuerLehrerDbContext> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        const int maxRetryAttempts = 30;
        const int maxRetryDelayInSeconds = 2;
        
        var attempt = 0;
        while (true)
        {
            try
            {
                attempt++;
                _logger.LogInformation("Versuche DB-Migration (Versuch {Attempt}/{Max})", attempt, maxRetryAttempts);
                await _context.Database.MigrateAsync();
                _logger.LogInformation("DB-Migration erfolgreich");
                return;
            }
            catch (MySqlException ex)
            {
                if (attempt >= maxRetryAttempts)
                {
                    _logger.LogError(ex, "DB-Migration fehlgeschlagen nach {Max} Versuchen", maxRetryAttempts);
                    throw;
                }
                
                await Task.Delay(TimeSpan.FromSeconds(maxRetryDelayInSeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler bei DB-Migration");
                throw;
            }
        }
    }
}
