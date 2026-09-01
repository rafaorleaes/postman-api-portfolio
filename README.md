<div align="center">

# 🧪 Suíte de Testes de API — Postman / Newman

[![API Tests (Newman)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml)
[![API Tests (C#)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/csharp-tests.yml)

**[📊 Ver relatório publicado](https://rafaorleaes.github.io/postman-api-portfolio/)**

</div>

Portfólio de QA de automação de API: uma coleção Postman executável contra a API pública [restful-api.dev](https://restful-api.dev), rodando em CI a cada push. Cobre fluxo CRUD encadeado com evidência de persistência, validação de contrato/schema, casos negativos, autenticação (login + 401, contra [dummyjson.com](https://dummyjson.com)) e data-driven testing via CSV. Tem também uma segunda suíte equivalente em **C# (xUnit + RestSharp)** em [`csharp-api-tests/`](csharp-api-tests).

`403`/IDOR não entram: exigem um backend com ownership real por usuário, que nenhuma API pública gratuita expõe.

## 📁 Estrutura

```
postman-api-portfolio/
├── collections/restful-api-crud.postman_collection.json
├── environments/       # dev.postman_environment.json, ci.postman_environment.json
├── data/objects.csv    # dados do run data-driven
├── csharp-api-tests/   # segunda suíte (xUnit + RestSharp)
├── .github/workflows/  # newman.yml, csharp-tests.yml
└── reports/            # gerado pelo CI (git-ignored)
```

## ▶️ Rodando localmente

```bash
npm install
npm test              # suíte principal
npm run test:ci       # com relatórios HTML + JUnit
npm run test:data     # pasta data-driven, iterando data/objects.csv
```

Suíte em C#: veja [`csharp-api-tests/README.md`](csharp-api-tests/README.md).

## 🔁 CI

`newman.yml` roda em todo push/PR, diariamente e sob demanda; publica os relatórios como artefato e no GitHub Pages. A `restful-api.dev` limita 50 requests/24h por IP, então o folder data-driven e a suíte C# completa só rodam no cron diário ou manualmente — push/PR ficam com a suíte principal, mais barata.

---

<div align="center">

Feito como peça de portfólio. Sinta-se à vontade para clonar e adaptar. ⭐

</div>
