using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Application.Dtos;

namespace TorneoGalactico.Application.Services;

public class RankingService(ISpeciesRepository speciesRepository, IBattleRepository battleRepository)
{
    public async Task<IReadOnlyList<RankingEntryDto>> GetAsync(CancellationToken ct = default)
    {
        var species = await speciesRepository.ListAsync(ct);
        var wins = await battleRepository.GetWinCountsAsync(ct);

        return species
            .Select(s => new { Species = s, Wins = wins.GetValueOrDefault(s.Id) })
            .OrderByDescending(x => x.Wins)
            .ThenBy(x => x.Species.Name, StringComparer.OrdinalIgnoreCase)
            .Select((x, index) => new RankingEntryDto(
                index + 1, x.Species.Id, x.Species.Name, x.Species.PowerLevel, x.Wins))
            .ToList();
    }
}