<div align="center">

# 🧪 Suíte de Testes de API — Postman / Newman

**Portfólio de QA de Automação de API**

[![API Tests (Newman)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml)
[![API Tests (C#)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml)
[![Postman Collection](https://img.shields.io/badge/Postman-v2.1.0-FF6C37?logo=postman&logoColor=white)](collections/restful-api-crud.postman_collection.json)
[![Newman](https://img.shields.io/badge/Newman-6.x-EF5350?logo=postman&logoColor=white)](https://www.npmjs.com/package/newman)
[![Node.js](https://img.shields.io/badge/Node.js-20+-339933?logo=node.js&logoColor=white)](https://nodejs.org)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](csharp-api-tests)
![License: MIT](https://img.shields.io/badge/License-MIT-informational)

**[📊 Ver último relatório publicado no GitHub Pages](https://rafaorleaes.github.io/postman-api-portfolio/)**

</div>

Uma coleção Postman executável, versionada e rodando em CI a cada push — contra a API pública [restful-api.dev](https://restful-api.dev), que persiste dados de verdade e permite demonstrar o ciclo completo de CRUD com evidência de persistência. Inclui também uma segunda suíte equivalente em **C# (xUnit + RestSharp)** em [`csharp-api-tests/`](csharp-api-tests), demonstrando a mesma cobertura em outro stack.

> 💡 **Por que essa API e não o Petstore?** O Petstore público reseta o estado e fica instável em pipeline, o que deixaria o badge de CI vermelho por motivo alheio ao teste. A `restful-api.dev` grava os objetos de fato, então dá para provar o padrão *write → GET de verificação* e manter o CI estável.
>
> ⚠️ **Trade-off conhecido:** a `restful-api.dev` limita 50 requests/24h por IP. Como duas suítes (Newman + xUnit) compartilham esse orçamento, os workflows rodam a suíte principal (mais barata) em todo push/PR, e reservam os cenários mais caros — data-driven e a suíte C# completa — para a execução agendada diária ou disparo manual. Ver [Integração contínua](#integração-contínua).

---

### 📑 Índice

- [O que este projeto demonstra](#o-que-este-projeto-demonstra)
- [Stack](#stack)
- [Estrutura](#estrutura)
- [Como rodar localmente](#como-rodar-localmente)
- [Integração contínua](#integração-contínua)
- [Cobertura de testes](#cobertura-de-testes)
- [Roadmap](#roadmap-próximos-passos)

---

## O que este projeto demonstra

- **Fluxo CRUD encadeado** — um objeto é criado, verificado, atualizado, verificado de novo e removido, passando o `id` entre requests via variável de ambiente.
- **Evidência de persistência** — todo write é seguido de um `GET` que confere se o dado gravado bate com o enviado (não confia só no status da escrita).
- **Validação de contrato / schema** — `pm.response.to.have.jsonSchema(...)` valida o shape da resposta, não apenas o status.
- **Casos negativos** — id inexistente e recurso removido retornam `404` com corpo de erro.
- **Autenticação real (login, 400, 401)** — contra [dummyjson.com](https://dummyjson.com): login feliz captura um JWT, senha errada devolve `400`, e acessar um recurso protegido sem token / com token inválido devolve `401` — com um controle positivo provando que o mesmo endpoint responde `200` quando o token é válido.
- **Data-driven testing** — a pasta *Data-Driven (CSV)* roda o mesmo fluxo criar → verificar → limpar uma vez por linha de [`data/objects.csv`](data/objects.csv), via `newman -d`.
- **Asserções não-funcionais** — tempo de resposta abaixo do limite.
- **Regra global "nunca 500"** — checagem no nível da coleção que roda após *cada* request (DRY), tratando erro de servidor sempre como falha.
- **Dados determinísticos e isolados** — nome/valores únicos por execução (timestamp), evitando colisão entre runs.
- **Configuração sem segredos** — base URLs parametrizadas; nenhum token real fica versionado.
- **CI/CD com publicação de relatório** — GitHub Actions executando a suíte a cada push/PR, mais uma execução diária agendada, publicando os relatórios HTML/JUnit como artefatos **e** no GitHub Pages.
- **Segunda vitrine em C#** — a mesma cobertura (fluxo CRUD, negativos, auth, data-driven) portada para xUnit + RestSharp em [`csharp-api-tests/`](csharp-api-tests), incluindo uma checagem global "nunca 500" via `DelegatingHandler` — o equivalente C# do event script de coleção do Postman.

## 🛠️ Stack

| Camada          | Ferramenta                          |
|-----------------|-------------------------------------|
| Testes de API   | Postman Collection v2.1.0           |
| Execução CLI    | Newman                              |
| Relatórios      | `htmlextra` (HTML) + JUnit (XML)    |
| CI/CD           | GitHub Actions + GitHub Pages       |
| Runtime (JS)    | Node.js 20                          |
| Segunda vitrine | .NET 8 · xUnit · RestSharp          |

## 📁 Estrutura

```
postman-api-portfolio/
├── collections/
│   └── restful-api-crud.postman_collection.json   # 4 pastas, 16 requests
├── environments/
│   ├── dev.postman_environment.json               # execução local
│   └── ci.postman_environment.json                # execução no pipeline
├── data/
│   └── objects.csv                                 # dados para o run data-driven
├── csharp-api-tests/                                # segunda vitrine (xUnit + RestSharp)
│   ├── Infra/                                        # RestClient factory + handler "nunca 500"
│   ├── Models/                                       # DTOs (System.Text.Json)
│   ├── CrudFlowTests.cs
│   ├── NegativeTests.cs
│   ├── AuthTests.cs
│   ├── DataDrivenTests.cs
│   └── ApiPortfolio.Tests.csproj
├── .github/workflows/
│   ├── newman.yml                                    # pipeline Postman/Newman + deploy Pages
│   └── csharp-tests.yml                              # pipeline da suíte C#
├── reports/                                          # relatórios gerados (git-ignored)
├── package.json                                      # scripts + dependências
├── package-lock.json
└── README.md
```

## ▶️ Como rodar localmente

Pré-requisito: Node.js 20+.

```bash
# 1. Instala as dependências (Newman + reporter HTML)
npm install

# 2. Roda a suíte principal com saída no terminal
npm test

# 3. Roda com relatórios HTML + JUnit (mesmo comando do CI)
npm run test:ci

# 4. Roda a pasta data-driven isolada, iterando data/objects.csv
npm run test:data
```

Os relatórios ficam em `reports/report.html` / `reports/junit.xml` (suíte principal) e `reports/report-data.html` / `reports/junit-data.xml` (data-driven).

Rodando o Newman direto, sem os scripts do npm:

```bash
npx newman run collections/restful-api-crud.postman_collection.json \
  -e environments/dev.postman_environment.json \
  --reporters cli,htmlextra
```

Para rodar a suíte em C# (xUnit + RestSharp), veja o [README de `csharp-api-tests/`](csharp-api-tests/README.md).

## 🔁 Integração contínua

O workflow [`.github/workflows/newman.yml`](.github/workflows/newman.yml) dispara em:

- `push` na `main`
- qualquer `pull_request`
- manualmente (`workflow_dispatch`)
- diariamente às 08:00 UTC (monitoramento agendado)

Cada execução instala as dependências com `npm ci`, roda a **suíte principal** e publica os relatórios HTML e JUnit como artefatos do build. O job `pages` baixa esse relatório e publica em **[rafaorleaes.github.io/postman-api-portfolio](https://rafaorleaes.github.io/postman-api-portfolio/)**.

A pasta **Data-Driven (CSV)** só roda no cron diário ou em `workflow_dispatch` — não em todo push/PR — para não estourar o limite de 50 requests/24h da `restful-api.dev` (ver aviso no topo do README). O mesmo racional se aplica ao [`csharp-tests.yml`](.github/workflows/csharp-tests.yml): compila em todo push/PR que toque `csharp-api-tests/**`, mas só roda a suíte completa (com chamadas reais) no cron ou manualmente.

## ✅ Cobertura de testes

As dimensões abaixo seguem o checklist mínimo de teste de API (feliz, persistência, negativo, auth, sem 500):

| Dimensão              | Coberto | Onde                                                        |
|-----------------------|:-------:|-------------------------------------------------------------|
| Caminho feliz         |   ✅    | POST/GET/PUT/DELETE do fluxo CRUD                           |
| Persistência          |   ✅    | GET de verificação após POST e após PUT                    |
| Negativo              |   ✅    | id inexistente → 404, recurso removido → 404               |
| Contrato / schema     |   ✅    | `jsonSchema` na criação e na listagem                       |
| Sem 500               |   ✅    | checagem global no nível da coleção                         |
| Autenticação (401)    |   ✅    | pasta *Autenticação*, contra dummyjson.com                  |
| Autorização (403/IDOR)|   ➖    | ver nota abaixo                                             |
| Data-driven           |   ✅    | pasta *Data-Driven (CSV)*, via `data/objects.csv`            |

**Nota honesta sobre 403/IDOR:** `restful-api.dev` não tem camada de auth, então os casos de `401` rodam contra [dummyjson.com](https://dummyjson.com) (login real via JWT). Já um `403` de autorização (token válido sem permissão) ou um IDOR (ler/escrever recurso privado de *outro* usuário) exigem um backend com *ownership* real por usuário — nenhuma API pública gratuita testada (dummyjson, reqres, GoREST) expõe isso sem uma conta própria com dados privados. O padrão de teste já usado aqui (capturar token, reusar em requests protegidos, ter um controle positivo) se estende para esses casos sem mudança estrutural assim que houver um backend assim disponível.

## 🗺️ Roadmap (próximos passos)

- [x] Apontar a suíte para uma API autenticada e adicionar os casos de `401` — feito contra dummyjson.com; `403`/IDOR seguem em aberto (ver nota honesta acima).
- [x] Publicar o relatório HTML no GitHub Pages — [rafaorleaes.github.io/postman-api-portfolio](https://rafaorleaes.github.io/postman-api-portfolio/).
- [x] Adicionar `data-driven testing` (iterações via arquivo CSV/JSON com `--iteration-data`) — pasta *Data-Driven (CSV)*.
- [x] Portar a mesma cobertura para C# (xUnit + RestSharp) como segunda vitrine — [`csharp-api-tests/`](csharp-api-tests).
- [ ] Testar a suíte C# de ponta a ponta contra a API real assim que a cota diária da `restful-api.dev` resetar (ver [`csharp-api-tests/README.md`](csharp-api-tests/README.md#status-de-verificação)).

---

<div align="center">

Feito como peça de portfólio. Sinta-se à vontade para clonar e adaptar. ⭐

</div>
