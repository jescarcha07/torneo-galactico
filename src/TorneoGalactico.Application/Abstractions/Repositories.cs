using TorneoGalactico.Domain;

namespace TorneoGalactico.Application.Abstractions;

public interface ISpeciesRepository
{
    Task AddAsync(Species species, CancellationToken ct = default);
    Task<Species?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNormalizedNameAsync(string normalizedName, CancellationToken ct = default);
    Task<IReadOnlyList<Species>> ListAsync(CancellationToken ct = default);
}

public interface IBattleRepository
{
    Task AddAsync(Battle battle, CancellationToken ct = default);
    Task<IReadOnlyList<Battle>> ListAsync(CancellationToken ct = default);

    /// <summary>Cantidad de victorias por especie (Id de especie → victorias).</summary>
    Task<IReadOnlyDictionary<Guid, int>> GetWinCountsAsync(CancellationToken ct = default);
}

/// <summary>Abstracción del azar para poder probar el combate aleatorio.</summary>
public interface IRandomGenerator
{
    int Next(int maxExclusive);
}