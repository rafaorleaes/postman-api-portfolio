<div align="center">

# 🧪 Suíte de Testes de API — C# / xUnit / RestSharp

**Segunda vitrine: mesma cobertura da [suíte Postman/Newman](../README.md), portada para .NET**

[![API Tests (C#)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![xUnit](https://img.shields.io/badge/xUnit-2.5-informational)](https://xunit.net)
[![RestSharp](https://img.shields.io/badge/RestSharp-112.x-blue)](https://restsharp.dev)

</div>

Mesmas duas APIs públicas da suíte Postman, mesma cobertura de dimensões (feliz, persistência, negativo, contrato, sem-500, autenticação, data-driven) — implementadas em C# com [xUnit](https://xunit.net) + [RestSharp](https://restsharp.dev) para mostrar o mesmo raciocínio de teste em outro stack.

## 📁 Estrutura

```
csharp-api-tests/
├── Infra/
│   ├── ApiClients.cs        # factory dos RestClient (restful-api.dev / dummyjson.com)
│   └── Never500Handler.cs   # DelegatingHandler: falha qualquer teste se a resposta for 5xx
├── Models/
│   ├── ObjectModels.cs      # DTOs do fluxo CRUD
│   └── AuthModels.cs        # DTOs de login/auth
├── CrudFlowTests.cs         # fluxo encadeado: cria → verifica → atualiza → verifica → remove → 404
├── NegativeTests.cs         # id inexistente (404) + contrato da listagem
├── AuthTests.cs             # login feliz/triste + 401 sem token / token inválido + controle positivo
├── DataDrivenTests.cs       # [Theory] + [MemberData], equivalente ao CSV do lado Postman
└── ApiPortfolio.Tests.csproj
```

## ▶️ Como rodar localmente

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd csharp-api-tests
dotnet restore
dotnet build          # so compila, nao chama nenhuma API
dotnet test           # roda a suite completa (chamadas reais)

# Rodar so um arquivo/classe:
dotnet test --filter "FullyQualifiedName~AuthTests"
```

## 🧩 Decisões de design que valem explicar

- **`Never500Handler`** — um `DelegatingHandler` plugado em todo `RestClient` via `RestClientOptions.ConfigureMessageHandler`. Ele intercepta *toda* resposta HTTP e falha o teste se vier `500`, sem precisar repetir a asserção em cada teste. É o equivalente C# do event script de nível de coleção no Postman (`pm.test('nao retornou 500', ...)`, que roda depois de cada request).
- **`AuthTests` não precisa reordenar nada** — cada `[Fact]` cria seu próprio `RestClient` (logo, seu próprio cookie jar), então o teste "sem token" nunca herda o cookie de sessão que o dummyjson define no login. No lado Postman isso exigiu reordenar a pasta (rodar os 401 antes do login) porque o Newman reusa cookies entre requests da mesma coleção — outra diferença interessante entre as duas ferramentas.
- **`DataDrivenTests`** usa `[Theory]` + `[MemberData]` em vez de ler `data/objects.csv` diretamente — é o idiomático em xUnit. Os valores espelham [`../data/objects.csv`](../data/objects.csv) manualmente; não há acoplamento de arquivo entre as duas suítes.

## ⚠️ Status de verificação

- `dotnet build`: ✅ compila limpo (0 erros, 0 avisos).
- `AuthTests` (contra dummyjson.com): ✅ os 5 testes rodam e passam de verdade.
- `CrudFlowTests`, `NegativeTests`, `DataDrivenTests` (contra restful-api.dev): escritos e revisados linha a linha contra os formatos de resposta reais (confirmados via curl e via a suíte Postman, que já passa 100% contra a mesma API), mas **ainda não confirmados por uma execução real aqui** — a suíte Postman já havia consumido a cota gratuita de 50 requests/24h da `restful-api.dev` antes deste projeto ser escrito. Rode `dotnet test` quando a cota resetar (ou com uma chave de API própria, criando conta grátis em restful-api.dev) para confirmar.

## 🗺️ Próximos passos

- [ ] Confirmar `CrudFlowTests` / `NegativeTests` / `DataDrivenTests` contra a API real (ver status acima).
- [ ] Gerar relatório HTML/coverage e publicar como artefato do `csharp-tests.yml`, no mesmo espírito do `htmlextra` do lado Newman.
