# BankMore - Plataforma de Banco Digital

BankMore é uma plataforma financeira baseada em microsserviços, desenvolvida com foco em escalabilidade, segurança e resiliência. O sistema foi construído seguindo os padrões de **DDD (Domain-Driven Design)** e **CQRS (Command Query Responsibility Segregation)**.

---

## Arquitetura

```
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
```

---

## Microsserviços

### API Conta Corrente

Responsável pelo cadastro, autenticação e operações financeiras da conta corrente.

| Endpoint                        | Método | Descrição                        |
| ------------------------------- | ------ | -------------------------------- |
| `/api/contacorrente/cadastrar`  | POST   | Cadastra uma nova conta corrente |
| `/api/contacorrente/login`      | POST   | Autentica e retorna token JWT    |
| `/api/contacorrente/inativar`   | PATCH  | Inativa uma conta corrente       |
| `/api/contacorrente/movimentar` | POST   | Realiza débito ou crédito        |
| `/api/contacorrente/saldo`      | GET    | Consulta o saldo atual           |

### API Transferência

Responsável pela transferência de valores entre contas da mesma instituição.

| Endpoint             | Método | Descrição                          |
| -------------------- | ------ | ---------------------------------- |
| `/api/transferencia` | POST   | Realiza transferência entre contas |

---

## Tecnologias Utilizadas

| Tecnologia   | Finalidade                 |
| ------------ | -------------------------- |
| .NET 8       | Framework principal        |
| Dapper       | Acesso ao banco de dados   |
| MediatR      | Padrão Mediator / CQRS     |
| SQLite       | Banco de dados             |
| JWT (Bearer) | Autenticação e autorização |
| Swagger      | Documentação das APIs      |
| Docker       | Containerização            |
| Kubernetes   | Orquestração (produção)    |

---

## Padrões e Decisões Técnicas

* **DDD** — Camadas separadas por responsabilidade
* **CQRS** — Separação de comandos e queries
* **Idempotência** — Uso de `requisicaoId` para evitar duplicidade
* **JWT** — Autenticação via token
* **Isolamento de dados** — Dados sensíveis não trafegam entre microsserviços
* **Resiliência** — Estorno automático em falhas de transferência

---

## Segurança

* JWT em todos os endpoints
* Hash BCrypt para senhas
* CPF isolado no microsserviço de origem
* Token com dados mínimos

---

## Como Executar

### Pré-requisitos

* .NET 8 SDK
* Docker
* Docker Compose

### Docker Compose

```bash
docker-compose up --build
```

### Swagger

* Conta Corrente: [http://localhost:7001/swagger](http://localhost:7001/swagger)
* Transferência: [http://localhost:7002/swagger](http://localhost:7002/swagger)

---

## Variáveis de Ambiente

| Variável                             | Descrição    |
| ------------------------------------ | ------------ |
| Jwt__Key                             | Chave JWT    |
| ConnectionStrings__DefaultConnection | Banco SQLite |

---

## Infraestrutura Kubernetes

```bash
kubectl apply -f k8s/
```

Cada serviço roda com **2 réplicas**.

---

## Regras de Negócio

### Conta Corrente

* Apenas contas válidas podem operar
* Apenas valores positivos

### Transferência

* Débito + crédito atômico
* Estorno automático em falhas
* Idempotência via `requisicaoId`

---

## Fluxo de Transferência

Cliente → Transferencia API → Conta Corrente API → Persistência → Estorno (se necessário)

---

# 🐳 Docker & Kubernetes (Containerização e Deploy)

O projeto BankMore foi preparado para execução em ambientes containerizados utilizando **Docker** e orquestrado via **Kubernetes**, garantindo escalabilidade e alta disponibilidade.

---

## 📦 Containerização com Docker

Cada microsserviço possui sua própria imagem Docker baseada em build multi-stage.

```text
src/Services/ContaCorrente/ContaCorrente.API
src/Services/Transferencia/Transferencia.API
```

Dockerfiles:

```text
docker/conta-corrente/Dockerfile
docker/transferencia/Dockerfile
```

---

## 🚀 Build das imagens

```bash
docker build -f docker/conta-corrente/Dockerfile -t mj2rsdockerpass/conta-corrente:latest .
docker build -f docker/transferencia/Dockerfile -t mj2rsdockerpass/transferencia:latest .
```

---

## 📤 Publicação das imagens

```bash
docker login
docker push mj2rsdockerpass/conta-corrente:latest
docker push mj2rsdockerpass/transferencia:latest
```

---

## ☸️ Kubernetes

* 2 réplicas por serviço
* Service Discovery interno
* Load balancing automático

Estrutura:

```text
k8s/
├── deployments
├── services
├── ingress
```

---

## 🔁 Deploy

```bash
kubectl apply -f k8s/
```

---

## 🌐 Ingress

```text
http://bankmore.local/conta
http://bankmore.local/transferencia
```

---

## ⚙️ Arquitetura

Usuário → Ingress → Service → Pod → API

---

## 📈 Escalabilidade

* 2 réplicas mínimas
* balanceamento automático
* isolamento por container

---

## 🔥 Benefícios

* Alta disponibilidade
* Escalabilidade horizontal
* Deploy independente
* Isolamento de falhas
* Ambiente padronizado
