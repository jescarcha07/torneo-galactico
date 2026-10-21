using TorneoGalactico.Domain;

namespace TorneoGalactico.Application.Dtos;

public record CreateSpeciesRequest(string Name, int PowerLevel, string SpecialAbility);

public record SpeciesDto(Guid Id, string Name, int PowerLevel, string SpecialAbility, DateTime CreatedAt)
{
    public static SpeciesDto From(Species s) =>
        new(s.Id, s.Name, s.PowerLevel, s.SpecialAbility, s.CreatedAt);
}

public record StartBattleRequest(Guid FighterAId, Guid FighterBId);

public record BattleDto(
    Guid Id,
    Guid FighterAId, string FighterAName,
    Guid FighterBId, string FighterBName,
    Guid WinnerId, string WinnerName,
    DateTime FoughtAt);

public record RankingEntryDto(int Position, Guid SpeciesId, string Name, int PowerLevel, int Wins);