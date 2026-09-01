using System.Text.Json.Serialization;

namespace ApiPortfolio.Tests.Models;

public class ObjectData
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }
}

public class CreateObjectRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("data")]
    public ObjectData Data { get; set; } = new();
}

public class ObjectResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("data")]
    public ObjectData? Data { get; set; }

    [JsonPropertyName("createdAt")]
    public long? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public long? UpdatedAt { get; set; }
}

public class ErrorResponse
{
    // restful-api.dev usa "error"; dummyjson usa "message" — um DTO cobre os dois formatos.
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
