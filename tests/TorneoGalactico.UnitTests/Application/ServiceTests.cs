using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Application.Common;
using TorneoGalactico.Application.Dtos;
using TorneoGalactico.Application.Services;
using TorneoGalactico.Domain;
using TorneoGalactico.Infrastructure.Persistence.InMemory;

namespace TorneoGalactico.UnitTests.Application;

public class ServiceTests
{
    private readonly InMemorySpeciesRepository _speciesRepo = new();
    private readonly InMemoryBattleRepository _battleRepo = new();

    private SpeciesService Species() => new(_speciesRepo);
    private BattleService Battles(params int[] randomValues) =>
        new(_speciesRepo, _battleRepo, new FixedRandom(randomValues));
    private RankingService Ranking() => new(_speciesRepo, _battleRepo);

    [Fact]
    public async Task Duplicate_name_is_rejected_ignoring_case()
    {
        await Species().CreateAsync(new CreateSpeciesRequest("Zorgon", 900, "Rayo"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            Species().CreateAsync(new CreateSpeciesRequest("ZORGON", 100, "Otra")));
    }

    [Fact]
    public async Task Battle_with_unknown_species_throws_not_found()
    {
        var zorgon = await Species().CreateAsync(new CreateSpeciesRequest("Zorgon", 900, "Rayo"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            Battles().StartAsync(new StartBattleRequest(zorgon.Id, Guid.NewGuid())));
    }

    [Fact]
    public async Task Ranking_is_ordered_by_wins_and_includes_species_without_wins()
    {
        var zorgon = await Species().CreateAsync(new CreateSpeciesRequest("Zorgon", 900, "Rayo"));
        var kryll = await Species().CreateAsync(new CreateSpeciesRequest("Kryll", 500, "Veneno"));
        await Species().CreateAsync(new CreateSpeciesRequest("Abyssal", 100, "Sombra"));

        await Battles().StartAsync(new StartBattleRequest(zorgon.Id, kryll.Id)); // gana Zorgon
        await Battles().StartAsync(new StartBattleRequest(kryll.Id, zorgon.Id)); // gana Zorgon

        var ranking = await Ranking().GetAsync();

        Assert.Equal(3, ranking.Count);
        Assert.Equal(("Zorgon", 2), (ranking[0].Name, ranking[0].Wins));
        Assert.Equal(0, ranking[1].Wins); // Abyssal y Kryll con 0, por orden alfabético
        Assert.Equal("Abyssal", ranking[1].Name);
        Assert.Equal("Kryll", ranking[2].Name);
    }

    [Fact]
    public async Task Random_battle_always_picks_two_different_species()
    {
        await Species().CreateAsync(new CreateSpeciesRequest("A1", 1, "x"));
        await Species().CreateAsync(new CreateSpeciesRequest("B2", 2, "x"));
        await Species().CreateAsync(new CreateSpeciesRequest("C3", 3, "x"));

        // first = 1, second = 1 → como second >= first, pasa a 2
        var battle = await Battles(1, 1).StartRandomAsync();

        Assert.NotEqual(battle.FighterAId, battle.FighterBId);
    }

    [Fact]
    public async Task Random_battle_needs_at_least_two_species()
    {
        await Species().CreateAsync(new CreateSpeciesRequest("Solo", 1, "x"));

        await Assert.ThrowsAsync<DomainException>(() => Battles(0, 0).StartRandomAsync());
    }

    private sealed class FixedRandom(params int[] values) : IRandomGenerator
    {
        private readonly Queue<int> _values = new(values);
        public int Next(int maxExclusive) => _values.Dequeue();
    }
}