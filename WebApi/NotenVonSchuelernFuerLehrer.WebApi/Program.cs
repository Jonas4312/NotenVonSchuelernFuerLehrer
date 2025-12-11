using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NotenVonSchuelernFuerLehrer.WebApi.Services;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<HashService>();
builder.Services.AddSingleton<TestdatenAnlegenService>();
builder.Services.AddSingleton<MigrationService>();

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))),
    };
});

var app = builder.Build();

//TODO: Nicht im Prod verwenden
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.MigrateDatabase();
await app.SeedTestData();

app.Run();