using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TorneoGalactico.Application.Dtos;

namespace TorneoGalactico.IntegrationTests;

public class TournamentApiTests
{
    // Cada test crea su propia aplicación: datos aislados (modo memoria por defecto)
    private static HttpClient CreateClient() => new WebApplicationFactory<Program>().CreateClient();

    private static async Task<SpeciesDto> CreateSpecies(HttpClient client, string name, int power)
    {
        var response = await client.PostAsJsonAsync("/api/species",
            new CreateSpeciesRequest(name, power, "Habilidad de prueba"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<SpeciesDto>())!;
    }

    [Fact]
    public async Task Full_flow_register_fight_and_rank()
    {
        var client = CreateClient();
        var zorgon = await CreateSpecies(client, "Zorgon", 900);
        var kryll = await CreateSpecies(client, "Kryll", 500);

        var battleResponse = await client.PostAsJsonAsync("/api/battles",
            new StartBattleRequest(zorgon.Id, kryll.Id));
        Assert.Equal(HttpStatusCode.Created, battleResponse.StatusCode);
        var battle = await battleResponse.Content.ReadFromJsonAsync<BattleDto>();
        Assert.Equal(zorgon.Id, battle!.WinnerId);

        var ranking = await client.GetFromJsonAsync<List<RankingEntryDto>>("/api/ranking");
        Assert.Equal("Zorgon", ranking![0].Name);
        Assert.Equal(1, ranking[0].Wins);
        Assert.Equal(0, ranking[1].Wins);
    }

    [Fact]
    public async Task Duplicate_name_returns_409()
    {
        var client = CreateClient();
        await CreateSpecies(client, "Zorgon", 900);

        var response = await client.PostAsJsonAsync("/api/species",
            new CreateSpeciesRequest("zorgon", 100, "Otra"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_species_returns_400()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/species",
            new CreateSpeciesRequest("", -1, ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Species_cannot_fight_itself_returns_400()
    {
        var client = CreateClient();
        var zorgon = await CreateSpecies(client, "Zorgon", 900);

        var response = await client.PostAsJsonAsync("/api/battles",
            new StartBattleRequest(zorgon.Id, zorgon.Id));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_species_returns_404()
    {
        var client = CreateClient();
        var zorgon = await CreateSpecies(client, "Zorgon", 900);

        var response = await client.PostAsJsonAsync("/api/battles",
            new StartBattleRequest(zorgon.Id, Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Random_battle_works_with_two_species()
    {
        var client = CreateClient();
        await CreateSpecies(client, "Zorgon", 900);
        await CreateSpecies(client, "Kryll", 500);

        var response = await client.PostAsync("/api/battles/random", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}