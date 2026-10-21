using TorneoGalactico.Application.Abstractions;

namespace TorneoGalactico.Infrastructure;

public class SystemRandomGenerator : IRandomGenerator
{
    public int Next(int maxExclusive) => Random.Shared.Next(maxExclusive);
}