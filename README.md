# BankMore - Plataforma de Banco Digital

BankMore é uma plataforma financeira baseada em microsserviços, desenvolvida com foco em escalabilidade, segurança e resiliência. O sistema foi construído seguindo os padrões de **DDD (Domain-Driven Design)** e **CQRS (Command Query Responsibility Segregation)**.

---

## Arquitetura

BankMore/
├── src/
│   └── Services/
│       ├── ContaCorrente/
│       │   ├── ContaCorrente.API
│       │   ├── ContaCorrente.Application
│       │   └── ContaCorrente.Infrastructure
│       └── Transferencia/
│           ├── Transferencia.API
│           ├── Transferencia.Application
│           └── Transferencia.Infrastructure
├── k8s/
├── docker-compose.yaml
└── BankMore.sln

---

## Microsserviços

### API Conta Corrente
Responsável pelo cadastro, autenticação e operações financeiras da conta corrente.

| Endpoint | Método | Descrição |
|---|---|---|
| `/api/contacorrente/cadastrar` | POST | Cadastra uma nova conta corrente |
| `/api/contacorrente/login` | POST | Autentica e retorna token JWT |
| `/api/contacorrente/inativar` | PATCH | Inativa uma conta corrente |
| `/api/contacorrente/movimentar` | POST | Realiza débito ou crédito |
| `/api/contacorrente/saldo` | GET | Consulta o saldo atual |

### API Transferência
Responsável pela transferência de valores entre contas da mesma instituição.

| Endpoint | Método | Descrição |
|---|---|---|
| `/api/transferencia` | POST | Realiza transferência entre contas |

---

## Tecnologias Utilizadas

| Tecnologia | Finalidade |
|---|---|
| .NET 8 | Framework principal |
| Dapper | Acesso ao banco de dados |
| MediatR | Padrão Mediator / CQRS |
| SQLite | Banco de dados |
| JWT (Bearer) | Autenticação e autorização |
| Swagger | Documentação das APIs |
| Docker | Containerização |
| Kubernetes | Orquestração (produção) |

---

## Padrões e Decisões Técnicas

- **DDD** — Cada microsserviço possui camadas bem definidas: API, Application e Infrastructure
- **CQRS** — Separação entre comandos (escrita) e queries (leitura) via MediatR
- **Idempotência** — Todas as requisições de movimentação e transferência são idempotentes via `requisicaoId`
- **JWT** — Todos os endpoints são protegidos, nenhum pode ser acessado sem token válido
- **Isolamento de dados** — Dados sensíveis como CPF trafegam apenas dentro do microsserviço de Conta Corrente
- **Resiliência** — Transferências possuem mecanismo de estorno automático em caso de falha

---

## Segurança

- Autenticação via **JWT Bearer Token** em todos os endpoints
- Senhas armazenadas com **hash BCrypt**
- CPF e dados sensíveis **não transitam** entre microsserviços
- Token contém apenas a identificação da conta corrente

---

## Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Subindo com Docker Compose

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/bankmore.git
cd bankmore

# Suba os containers
docker-compose up --build
```

### Acessando os Swaggers

| Serviço | URL |
|---|---|
| ContaCorrente API | http://localhost:7001/swagger |
| Transferencia API | http://localhost:7002/swagger |

### Executando localmente (sem Docker)

```bash
# ContaCorrente
cd src/Services/ContaCorrente/ContaCorrente.API
dotnet run

# Transferencia (em outro terminal)
cd src/Services/Transferencia/Transferencia.API
dotnet run
```

---

## Variáveis de Ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `Jwt__Key` | Chave secreta para geração do JWT | `minha-chave-secreta` |
| `ConnectionStrings__DefaultConnection` | String de conexão SQLite | `Data Source=/data/banco.db` |
| `Services__ContaCorrente__BaseUrl` | URL base da API ContaCorrente | `http://contacorrente-api` |

---

## Infraestrutura (Produção)

O projeto inclui manifestos Kubernetes prontos para deploy em produção:

```bash
# Aplicar todos os manifestos
kubectl apply -f k8s/jwt-secret.yaml
kubectl apply -f k8s/contacorrente-pvc.yaml
kubectl apply -f k8s/transferencia-pvc.yaml
kubectl apply -f k8s/contacorrente-deployment.yaml
kubectl apply -f k8s/transferencia-deployment.yaml
```

Cada microsserviço roda com **mínimo de 2 réplicas** garantindo alta disponibilidade.

---

## Regras de Negócio

### Movimentação
- Apenas contas **cadastradas** podem receber movimentação (`INVALID_ACCOUNT`)
- Apenas contas **ativas** podem receber movimentação (`INACTIVE_ACCOUNT`)
- Apenas **valores positivos** são aceitos (`INVALID_VALUE`)
- Apenas os tipos **Crédito (C)** ou **Débito (D)** são aceitos (`INVALID_TYPE`)
- Apenas **Crédito** é permitido para contas diferentes da conta logada (`INVALID_TYPE`)

### Transferência
- Realiza **débito** na conta de origem
- Realiza **crédito** na conta de destino
- Em caso de falha no crédito, realiza **estorno automático** na conta de origem
- Requisições são **idempotentes** via `requisicaoId`

---

## Fluxo de Transferência

Cliente
│
▼
Transferencia API
├── Débito → ContaCorrente API ✓
├── Crédito → ContaCorrente API ✓
│     └── Falha → Estorno → ContaCorrente API
└── Persiste registro da transferência

---

## Licença

Este projeto foi desenvolvido como desafio técnico para a **BankMore Fintech**.
