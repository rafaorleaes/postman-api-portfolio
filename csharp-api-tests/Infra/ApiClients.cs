using RestSharp;

namespace ApiPortfolio.Tests.Infra;

public static class ApiClients
{
    // restful-api.dev: mesma API do fluxo CRUD principal (Postman/Newman).
    public static RestClient RestfulApiDev() => new(new RestClientOptions("https://api.restful-api.dev")
    {
        ConfigureMessageHandler = handler => new Never500Handler(handler)
    });

    // dummyjson.com: usada so para os casos de autenticacao (login + 401), que
    // restful-api.dev nao suporta por nao ter camada de auth.
    public static RestClient DummyJson() => new(new RestClientOptions("https://dummyjson.com")
    {
        ConfigureMessageHandler = handler => new Never500Handler(handler)
    });
}
