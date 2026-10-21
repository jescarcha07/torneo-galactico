using TorneoGalactico.Application.Abstractions;
using TorneoGalactico.Application.Common;
using TorneoGalactico.Application.Dtos;
using TorneoGalactico.Domain;

namespace TorneoGalactico.Application.Services;

public class SpeciesService(ISpeciesRepository repository)
{
    public async Task<SpeciesDto> CreateAsync(CreateSpeciesRequest request, CancellationToken ct = default)
    {
        var species = Species.Create(request.Name, request.PowerLevel, request.SpecialAbility);

        if (await repository.ExistsByNormalizedNameAsync(species.NormalizedName, ct))
            throw new ConflictException($"Ya existe una especie llamada '{species.Name}'.");

        await repository.AddAsync(species, ct);
        return SpeciesDto.From(species);
    }

    public async Task<SpeciesDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var species = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe la especie {id}.");
        return SpeciesDto.From(species);
    }

    public async Task<IReadOnlyList<SpeciesDto>> ListAsync(CancellationToken ct = default)
    {
        var all = await repository.ListAsync(ct);
        return all
            .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .Select(SpeciesDto.From)
            .ToList();
    }
}