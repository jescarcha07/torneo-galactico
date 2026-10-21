using System.Collections.Concurrent;
using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Domain;

namespace TorneoGalactico.Infrastructure.Persistence.InMemory;

public class InMemorySpeciesRepository : ISpeciesRepository
{
    private readonly ConcurrentDictionary<Guid, Species> _items = new();

    public Task AddAsync(Species species, CancellationToken ct = default)
    {
        if (!_items.TryAdd(species.Id, species))
            throw new InvalidOperationException($"Ya existe una especie con Id {species.Id}.");
        return Task.CompletedTask;
    }

    public Task<Species?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _items.TryGetValue(id, out var species);
        return Task.FromResult(species);
    }

    public Task<bool> ExistsByNormalizedNameAsync(string normalizedName, CancellationToken ct = default) =>
        Task.FromResult(_items.Values.Any(s => s.NormalizedName == normalizedName));

    public Task<IReadOnlyList<Species>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Species>>(_items.Values.OrderBy(s => s.CreatedAt).ToList());
}