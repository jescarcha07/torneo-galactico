using System.Collections.Concurrent;
using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Domain;

namespace TorneoGalactico.Infrastructure.Persistence.InMemory;

public class InMemoryBattleRepository : IBattleRepository
{
    private readonly ConcurrentQueue<Battle> _items = new();

    public Task AddAsync(Battle battle, CancellationToken ct = default)
    {
        _items.Enqueue(battle);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Battle>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Battle>>(_items.ToList());

    public Task<IReadOnlyDictionary<Guid, int>> GetWinCountsAsync(CancellationToken ct = default)
    {
        IReadOnlyDictionary<Guid, int> counts = _items
            .GroupBy(b => b.WinnerId)
            .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult(counts);
    }
}