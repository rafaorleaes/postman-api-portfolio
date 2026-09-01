using System.Net;
using ApiPortfolio.Tests.Infra;
using ApiPortfolio.Tests.Models;
using RestSharp;

namespace ApiPortfolio.Tests;

// Porta do fluxo "Ciclo CRUD (fluxo encadeado)" da colecao Postman: cria, verifica
// persistencia, atualiza, verifica de novo e remove, tudo em um unico teste
// (assim como a pasta encadeada no Postman/Newman).
public class CrudFlowTests
{
    [Fact]
    public async Task Fluxo_Crud_Completo_Cria_Verifica_Atualiza_Verifica_Remove()
    {
        using var client = ApiClients.RestfulApiDev();
        var nomeOriginal = $"QA Portfolio Item {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        // 1. POST cria objeto -> 200
        var createRequest = new RestRequest("objects", Method.Post)
            .AddJsonBody(new CreateObjectRequest
            {
                Name = nomeOriginal,
                Data = new ObjectData { Color = "Verde", Year = 2026 }
            });
        var createResponse = await client.ExecuteAsync<ObjectResponse>(createRequest);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.NotNull(createResponse.Data);
        Assert.False(string.IsNullOrWhiteSpace(createResponse.Data!.Id));
        Assert.Equal(nomeOriginal, createResponse.Data!.Name);

        var id = createResponse.Data!.Id;

        // 2. GET verifica persistencia -> 200
        var getRequest1 = new RestRequest($"objects/{id}", Method.Get);
        var getResponse1 = await client.ExecuteAsync<ObjectResponse>(getRequest1);

        Assert.Equal(HttpStatusCode.OK, getResponse1.StatusCode);
        Assert.Equal(nomeOriginal, getResponse1.Data!.Name);
        Assert.Equal("Verde", getResponse1.Data!.Data!.Color);

        // 3. PUT atualiza objeto -> 200
        var nomeAtualizado = $"QA Portfolio Item ATUALIZADO {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var putRequest = new RestRequest($"objects/{id}", Method.Put)
            .AddJsonBody(new CreateObjectRequest
            {
                Name = nomeAtualizado,
                Data = new ObjectData { Color = "Azul", Year = 2027 }
            });
        var putResponse = await client.ExecuteAsync<ObjectResponse>(putRequest);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        Assert.NotNull(putResponse.Data!.UpdatedAt);
        Assert.Equal(nomeAtualizado, putResponse.Data!.Name);

        // 4. GET verifica atualizacao -> 200
        var getRequest2 = new RestRequest($"objects/{id}", Method.Get);
        var getResponse2 = await client.ExecuteAsync<ObjectResponse>(getRequest2);

        Assert.Equal(HttpStatusCode.OK, getResponse2.StatusCode);
        Assert.Equal(nomeAtualizado, getResponse2.Data!.Name);
        Assert.Equal("Azul", getResponse2.Data!.Data!.Color);

        // 5. DELETE remove objeto -> 200
        var deleteRequest = new RestRequest($"objects/{id}", Method.Delete);
        var deleteResponse = await client.ExecuteAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Contains("deleted", deleteResponse.Content ?? "", StringComparison.OrdinalIgnoreCase);

        // 6. GET confirma remocao -> 404
        var getRequest3 = new RestRequest($"objects/{id}", Method.Get);
        var getResponse3 = await client.ExecuteAsync<ErrorResponse>(getRequest3);

        Assert.Equal(HttpStatusCode.NotFound, getResponse3.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(getResponse3.Data?.Error));
    }
}
