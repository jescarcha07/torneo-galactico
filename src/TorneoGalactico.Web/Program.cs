using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Application.Services;
using TorneoGalactico.Infrastructure;
using TorneoGalactico.Infrastructure.Persistence.InMemory;
using TorneoGalactico.Web;

var builder = WebApplication.CreateBuilder(args);

// Persistencia (en el paso 8 se vuelve configurable)
builder.Services.AddSingleton<ISpeciesRepository, InMemorySpeciesRepository>();
builder.Services.AddSingleton<IBattleRepository, InMemoryBattleRepository>();
builder.Services.AddSingleton<IRandomGenerator, SystemRandomGenerator>();

// Casos de uso
builder.Services.AddScoped<SpeciesService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<RankingService>();

// Errores
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5055").AllowAnyHeader().AllowAnyMethod()));


var app = builder.Build();
// después de builder.Build():
app.UseCors();
app.UseExceptionHandler();
app.UseDefaultFiles();   // sirve index.html en "/"
app.UseStaticFiles();    // sirve wwwroot

app.MapTournamentEndpoints();

app.Run();

public partial class Program { }