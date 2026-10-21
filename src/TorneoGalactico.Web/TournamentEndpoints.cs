using TorneoGalactico.Application.Dtos;
using TorneoGalactico.Application.Services;

namespace TorneoGalactico.Web;

public static class TournamentEndpoints
{
    public static IEndpointRouteBuilder MapTournamentEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        // Especies
        api.MapPost("/species", async (CreateSpeciesRequest request, SpeciesService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(request, ct);
            return Results.Created($"/api/species/{created.Id}", created);
        });

        api.MapGet("/species", (SpeciesService service, CancellationToken ct) => service.ListAsync(ct));

        api.MapGet("/species/{id:guid}", (Guid id, SpeciesService service, CancellationToken ct) =>
            service.GetAsync(id, ct));

        // Combates
        api.MapPost("/battles", async (StartBattleRequest request, BattleService service, CancellationToken ct) =>
        {
            var battle = await service.StartAsync(request, ct);
            return Results.Created((string?)null, battle);
        });

        api.MapPost("/battles/random", async (BattleService service, CancellationToken ct) =>
        {
            var battle = await service.StartRandomAsync(ct);
            return Results.Created((string?)null, battle);
        });

        api.MapGet("/battles", (BattleService service, CancellationToken ct) => service.ListAsync(ct));

        // Ranking
        api.MapGet("/ranking", (RankingService service, CancellationToken ct) => service.GetAsync(ct));

        return app;
    }
}