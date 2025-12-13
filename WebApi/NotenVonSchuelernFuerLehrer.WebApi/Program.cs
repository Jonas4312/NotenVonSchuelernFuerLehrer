using NotenVonSchuelernFuerLehrer.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguredController();
builder.Services.AddConfiguredSwaggerGen();
builder.Services.AddConfiguredAuthorization(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddDomainRepositories(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddWebApiConfigurations(builder.Configuration);
builder.Services.AddRequestHandlers();

var app = builder.Build();

await app.MigrateDatabase();
await app.SeedTestData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseConfiguredExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();