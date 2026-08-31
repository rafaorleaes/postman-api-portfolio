<div align="center">

# 🧪 Suíte de Testes de API — Postman / Newman

**Portfólio de QA de Automação de API**

[![API Tests (Newman)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml/badge.svg)](https://github.com/rafaorleaes/postman-api-portfolio/actions/workflows/newman.yml)
[![Postman Collection](https://img.shields.io/badge/Postman-v2.1.0-FF6C37?logo=postman&logoColor=white)](collections/restful-api-crud.postman_collection.json)
[![Newman](https://img.shields.io/badge/Newman-6.x-EF5350?logo=postman&logoColor=white)](https://www.npmjs.com/package/newman)
[![Node.js](https://img.shields.io/badge/Node.js-20+-339933?logo=node.js&logoColor=white)](https://nodejs.org)
![License: MIT](https://img.shields.io/badge/License-MIT-informational)

</div>

Uma coleção Postman executável, versionada e rodando em CI a cada push — contra a API pública [restful-api.dev](https://restful-api.dev), que persiste dados de verdade e permite demonstrar o ciclo completo de CRUD com evidência de persistência.

> 💡 **Por que essa API e não o Petstore?** O Petstore público reseta o estado e fica instável em pipeline, o que deixaria o badge de CI vermelho por motivo alheio ao teste. A `restful-api.dev` grava os objetos de fato, então dá para provar o padrão *write → GET de verificação* e manter o CI estável.

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
- **Asserções não-funcionais** — tempo de resposta abaixo do limite.
- **Regra global "nunca 500"** — checagem no nível da coleção que roda após *cada* request (DRY), tratando erro de servidor sempre como falha.
- **Dados determinísticos e isolados** — nome/valores únicos por execução (timestamp), evitando colisão entre runs.
- **Configuração sem segredos** — base URL parametrizada; a variável `token` fica vazia no arquivo versionado e seria injetada pelo CI.
- **CI/CD** — GitHub Actions executando a suíte a cada push/PR, mais uma execução diária agendada, com relatórios HTML e JUnit como artefatos.

## 🛠️ Stack

| Camada        | Ferramenta                          |
|---------------|-------------------------------------|
| Testes de API | Postman Collection v2.1.0           |
| Execução CLI  | Newman                              |
| Relatórios    | `htmlextra` (HTML) + JUnit (XML)    |
| CI/CD         | GitHub Actions                      |
| Runtime       | Node.js 20                          |

## 📁 Estrutura

```
postman-api-portfolio/
├── collections/
│   └── restful-api-crud.postman_collection.json   # 8 requests, 2 pastas
├── environments/
│   ├── dev.postman_environment.json               # execução local
│   └── ci.postman_environment.json                # execução no pipeline
├── .github/workflows/
│   └── newman.yml                                  # pipeline de CI
├── reports/                                        # relatórios gerados (git-ignored)
├── package.json                                    # scripts + dependências
├── package-lock.json
└── README.md
```

## ▶️ Como rodar localmente

Pré-requisito: Node.js 20+.

```bash
# 1. Instala as dependências (Newman + reporter HTML)
npm install

# 2. Roda a suíte com saída no terminal
npm test

# 3. Roda com relatórios HTML + JUnit (mesmo comando do CI)
npm run test:ci
```

Os relatórios ficam em `reports/report.html` e `reports/junit.xml`.

Rodando o Newman direto, sem os scripts do npm:

```bash
npx newman run collections/restful-api-crud.postman_collection.json \
  -e environments/dev.postman_environment.json \
  --reporters cli,htmlextra
```

## 🔁 Integração contínua

O workflow [`.github/workflows/newman.yml`](.github/workflows/newman.yml) dispara em:

- `push` na `main`
- qualquer `pull_request`
- manualmente (`workflow_dispatch`)
- diariamente às 08:00 UTC (monitoramento agendado)

Cada execução instala as dependências com `npm ci`, roda a suíte e publica os relatórios HTML e JUnit como artefatos do build.

## ✅ Cobertura de testes

As dimensões abaixo seguem o checklist mínimo de teste de API (feliz, persistência, negativo, auth, sem 500):

| Dimensão              | Coberto | Onde                                                        |
|-----------------------|:-------:|-------------------------------------------------------------|
| Caminho feliz         |   ✅    | POST/GET/PUT/DELETE do fluxo CRUD                           |
| Persistência          |   ✅    | GET de verificação após POST e após PUT                    |
| Negativo              |   ✅    | id inexistente → 404, recurso removido → 404               |
| Contrato / schema     |   ✅    | `jsonSchema` na criação e na listagem                       |
| Sem 500               |   ✅    | checagem global no nível da coleção                         |
| Autenticação          |   ➖    | ver nota abaixo                                             |

**Nota honesta sobre autenticação:** a `restful-api.dev` é uma API pública sem camada de auth, então cenários de `401` (sem token) e `403`/IDOR (token de outro usuário) não têm como ser exercitados aqui. O projeto já deixa o padrão pronto: a variável `token` existe nos environments (marcada como *secret* e vazia) e seria injetada no pipeline via `newman ... --env-var "token=$API_TOKEN"`, com o header `Authorization: Bearer {{token}}` nas requests. Contra uma API autenticada, esses casos entram sem mudança estrutural.

## 🗺️ Roadmap (próximos passos)

- [ ] Apontar a suíte para uma API autenticada e adicionar os casos de `401` / `403` / IDOR.
- [ ] Publicar o relatório HTML no GitHub Pages.
- [ ] Adicionar `data-driven testing` (iterações via arquivo CSV/JSON com `--iteration-data`).
- [ ] Portar a mesma cobertura para C# (xUnit + RestSharp) como segunda vitrine.

---

<div align="center">

Feito como peça de portfólio. Sinta-se à vontade para clonar e adaptar. ⭐

</div>
