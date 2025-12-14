using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<JwtService>();
        services.AddSingleton<HashService>();
        services.AddScoped<RequestExecutor>();
        services.AddScoped<MigrationService>();
        services.AddScoped<LehrerAccessor>();
        services.AddScoped<SeedDataService>();
        services.AddHttpClient<SeedDataService>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(configuration["Http:PicsumPhotos:BaseUrl"] ?? throw new InvalidOperationException("PicsumPhotos BaseUrl is not configured."));
            httpClient.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Http:PicsumPhotos:TimeoutInSeconds"));
        });
    }
    
    public static void AddWebApiConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfiguration>(configuration.GetSection("Jwt"));
        services.Configure<NoteConfiguration>(configuration.GetSection("Note"));
    }
    
    public static void AddDomainRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotenVonSchuelernFuerLehrerDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("NotenVonSchuelernFuerLehrerConnectionString");
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            options.UseMySQL(connectionString);
        });
        services.AddScoped<FachRepository>();
        services.AddScoped<KlasseRepository>();
        services.AddScoped<LehrerRepository>();
        services.AddScoped<NoteRepository>();
        services.AddScoped<SchuelerRepository>();
    }
    
    public static void AddRequestHandlers(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler, AenderNoteRequestHandler>();
        services.AddScoped<IRequestHandler, AenderSchuelerRequestHandler>();
        services.AddScoped<IRequestHandler, ErstelleNoteRequestHandler>();
        services.AddScoped<IRequestHandler, ErstelleSchuelerRequestHandler>();
        services.AddScoped<IRequestHandler, LadeKlassenEinesLehrersRequestHandler>();
        services.AddScoped<IRequestHandler, LadeLehrerRequestHandler>();
        services.AddScoped<IRequestHandler, LadeNoteEinesSchuelersRequestHandler>();
        services.AddScoped<IRequestHandler, LadeNotenEinesSchuelersRequestHandler>();
        services.AddScoped<IRequestHandler, LadeSchuelerEinerKlasseRequestHandler>();
        services.AddScoped<IRequestHandler, LadeSchuelerRequestHandler>();
        services.AddScoped<IRequestHandler, LoescheNoteRequestHandler>();
        services.AddScoped<IRequestHandler, LoescheSchuelerRequestHandler>();
        services.AddScoped<IRequestHandler, LoginRequestHandler>();
    }

    public static void AddConfiguredAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))),
                };
            });
    }
    
    public static void AddConfiguredController(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new ApiResponse
                    {
                        Success = false,
                        Errors = context.ModelState
                            .SelectMany(e => e.Value?.Errors ?? [])
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                };
            });   
    }

    public static void AddConfiguredSwaggerGen(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", document), [] }
            });
        });
    }
}