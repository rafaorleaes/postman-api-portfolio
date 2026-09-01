using System.Net;
using ApiPortfolio.Tests.Infra;
using ApiPortfolio.Tests.Models;
using RestSharp;

namespace ApiPortfolio.Tests;

// Porta da pasta "Autenticacao (401 - dummyjson.com)" da colecao Postman.
// Cada teste cria seu proprio RestClient, entao nao ha contaminacao de cookie de
// sessao entre casos (diferente do Newman, aqui nao precisamos ordenar os testes).
//
// Nota honesta sobre 403/IDOR: ver README — exige um backend com ownership real
// por usuario, que nenhuma API publica gratuita disponibiliza.
public class AuthTests
{
    [Fact]
    public async Task Login_Valido_Retorna200_EAccessToken()
    {
        using var client = ApiClients.DummyJson();
        var request = new RestRequest("auth/login", Method.Post)
            .AddJsonBody(new LoginRequest { Username = "emilys", Password = "emilyspass" });

        var response = await client.ExecuteAsync<LoginResponse>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(response.Data?.AccessToken));
        Assert.Equal("emilys", response.Data!.Username);
    }

    [Fact]
    public async Task Login_Invalido_Retorna400()
    {
        using var client = ApiClients.DummyJson();
        var request = new RestRequest("auth/login", Method.Post)
            .AddJsonBody(new LoginRequest { Username = "emilys", Password = "senha-errada" });

        var response = await client.ExecuteAsync<ErrorResponse>(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(response.Data?.Message));
    }

    [Fact]
    public async Task AuthMe_SemToken_Retorna401()
    {
        using var client = ApiClients.DummyJson();
        var request = new RestRequest("auth/me", Method.Get);

        var response = await client.ExecuteAsync<ErrorResponse>(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthMe_TokenInvalido_Retorna401()
    {
        using var client = ApiClients.DummyJson();
        var request = new RestRequest("auth/me", Method.Get)
            .AddHeader("Authorization", "Bearer token-invalido-abc123");

        var response = await client.ExecuteAsync<ErrorResponse>(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthMe_TokenValido_Retorna200_ControlePositivo()
    {
        using var client = ApiClients.DummyJson();

        var loginRequest = new RestRequest("auth/login", Method.Post)
            .AddJsonBody(new LoginRequest { Username = "emilys", Password = "emilyspass" });
        var loginResponse = await client.ExecuteAsync<LoginResponse>(loginRequest);

        var meRequest = new RestRequest("auth/me", Method.Get)
            .AddHeader("Authorization", $"Bearer {loginResponse.Data!.AccessToken}");
        var meResponse = await client.ExecuteAsync<LoginResponse>(meRequest);

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        Assert.Equal("emilys", meResponse.Data!.Username);
    }
}
