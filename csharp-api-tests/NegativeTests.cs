using System.Net;
using ApiPortfolio.Tests.Infra;
using ApiPortfolio.Tests.Models;
using RestSharp;

namespace ApiPortfolio.Tests;

// Porta da pasta "Negativos & Contrato" da colecao Postman.
public class NegativeTests
{
    [Fact]
    public async Task GET_IdInexistente_Retorna404()
    {
        using var client = ApiClients.RestfulApiDev();
        var request = new RestRequest("objects/id-inexistente-999999999", Method.Get);

        var response = await client.ExecuteAsync<ErrorResponse>(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(response.Data?.Error));
    }

    [Fact]
    public async Task GET_Lista_Retorna200_EContratoMinimo()
    {
        using var client = ApiClients.RestfulApiDev();
        var request = new RestRequest("objects", Method.Get);

        var response = await client.ExecuteAsync<List<ObjectResponse>>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data!);
        Assert.False(string.IsNullOrWhiteSpace(response.Data![0].Id));
        Assert.False(string.IsNullOrWhiteSpace(response.Data![0].Name));
    }
}
