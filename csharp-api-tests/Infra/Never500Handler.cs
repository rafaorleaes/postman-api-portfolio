using System.Net;

namespace ApiPortfolio.Tests.Infra;

// Replica a checagem global "nunca 500" da colecao Postman: roda em toda resposta
// que passar por este handler e falha o teste se o servidor devolver 5xx.
public sealed class Never500Handler : DelegatingHandler
{
    public Never500Handler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        return response;
    }
}
