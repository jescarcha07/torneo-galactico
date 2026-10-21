using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Application.Common;
using TorneoGalactico.Application.Dtos;
using TorneoGalactico.Domain;

namespace TorneoGalactico.Application.Services;

public class BattleService(
    ISpeciesRepository speciesRepository,
    IBattleRepository battleRepository,
    IRandomGenerator random)
{
    public async Task<BattleDto> StartAsync(StartBattleRequest request, CancellationToken ct = default)
    {
        var fighterA = await speciesRepository.GetByIdAsync(request.FighterAId, ct)
            ?? throw new NotFoundException($"No existe la especie {request.FighterAId}.");
        var fighterB = await speciesRepository.GetByIdAsync(request.FighterBId, ct)
            ?? throw new NotFoundException($"No existe la especie {request.FighterBId}.");

        return await FightAsync(fighterA, fighterB, ct);
    }

    public async Task<BattleDto> StartRandomAsync(CancellationToken ct = default)
    {
        var all = await speciesRepository.ListAsync(ct);
        if (all.Count < 2)
            throw new DomainException("Se necesitan al menos dos especies para un combate aleatorio.");

        // Elige dos índices distintos sin reintentos:
        // el segundo se toma entre los N-1 restantes y se "salta" el primero.
        var first = random.Next(all.Count);
        var second = random.Next(all.Count - 1);
        if (second >= first) second++;

        return await FightAsync(all[first], all[second], ct);
    }

    public async Task<IReadOnlyList<BattleDto>> ListAsync(CancellationToken ct = default)
    {
        var names = (await speciesRepository.ListAsync(ct)).ToDictionary(s => s.Id, s => s.Name);
        var battles = await battleRepository.ListAsync(ct);

        return battles
            .OrderByDescending(b => b.FoughtAt)
            .Select(b => ToDto(b, names[b.FighterAId], names[b.FighterBId], names[b.WinnerId]))
            .ToList();
    }

    private async Task<BattleDto> FightAsync(Species fighterA, Species fighterB, CancellationToken ct)
    {
        var battle = Battle.Fight(fighterA, fighterB);   // la regla vive en el dominio
        await battleRepository.AddAsync(battle, ct);     // se registra el resultado

        var winnerName = battle.WinnerId == fighterA.Id ? fighterA.Name : fighterB.Name;
        return ToDto(battle, fighterA.Name, fighterB.Name, winnerName);
    }

    private static BattleDto ToDto(Battle b, string aName, string bName, string winnerName) =>
        new(b.Id, b.FighterAId, aName, b.FighterBId, bName, b.WinnerId, winnerName, b.FoughtAt);
}