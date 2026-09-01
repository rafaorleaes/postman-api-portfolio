using System.Net;
using ApiPortfolio.Tests.Infra;
using ApiPortfolio.Tests.Models;
using RestSharp;

namespace ApiPortfolio.Tests;

// Equivalente ao "Data-Driven (CSV)" do Newman: xUnit nao le CSV nativamente, entao
// o idiomatico e [Theory] + [MemberData]. Os valores espelham data/objects.csv
// (../data/objects.csv) do lado Postman.
public class DataDrivenTests
{
    public static IEnumerable<object[]> Objetos =>
        new List<object[]>
        {
            new object[] { "Notebook Dell XPS", "Prata", 2023 },
            new object[] { "Monitor LG UltraWide", "Preto", 2022 },
            new object[] { "Mouse Logitech MX", "Branco", 2024 },
        };

    [Theory]
    [MemberData(nameof(Objetos))]
    public async Task Cria_Verifica_E_Remove_Objeto(string nome, string cor, int ano)
    {
        using var client = ApiClients.RestfulApiDev();

        var createRequest = new RestRequest("objects", Method.Post)
            .AddJsonBody(new CreateObjectRequest { Name = nome, Data = new ObjectData { Color = cor, Year = ano } });
        var createResponse = await client.ExecuteAsync<ObjectResponse>(createRequest);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(nome, createResponse.Data!.Name);
        var id = createResponse.Data!.Id;

        var getRequest = new RestRequest($"objects/{id}", Method.Get);
        var getResponse = await client.ExecuteAsync<ObjectResponse>(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(cor, getResponse.Data!.Data!.Color);
        Assert.Equal(ano, getResponse.Data!.Data!.Year);

        var deleteRequest = new RestRequest($"objects/{id}", Method.Delete);
        var deleteResponse = await client.ExecuteAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}
