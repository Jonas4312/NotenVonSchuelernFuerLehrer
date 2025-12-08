using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<NotenVonSchuelernFuerLehrerDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("NotenVonSchuelernFuerLehrerConnectionString");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
    options.UseMySQL(connectionString);
});
builder.Services.AddScoped<BerechtigungRepository>();
builder.Services.AddScoped<FachRepository>();
builder.Services.AddScoped<KlasseRepository>();
builder.Services.AddScoped<LehrerRepository>();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<SchuelerRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.MigrateDatabase();

app.Run();