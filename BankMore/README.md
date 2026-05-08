# BankMore - Plataforma de Banco Digital

BankMore é uma plataforma financeira baseada em microsserviços, desenvolvida com foco em escalabilidade, segurança e resiliência. O sistema foi construído seguindo os padrões de **DDD (Domain-Driven Design)** e **CQRS (Command Query Responsibility Segregation)**.

---

# 🏦 O Banco Digital da Ana

A Ana é fundadora de uma fintech chamada **BankMore**.  
Ela contratou uma equipe de desenvolvedores para criar uma plataforma baseada em microsserviços, com foco em:

- Escalabilidade
- Segurança
- Resiliência
- Alta disponibilidade

---

# 📋 Requisitos do Sistema

O sistema inicial deve possuir as seguintes funcionalidades principais:

- ✅ Cadastro e autenticação de usuários
- ✅ Realização de movimentações em conta corrente:
  - Depósitos
  - Saques
- ✅ Transferências entre contas da mesma instituição
- ✅ Consulta de saldo

---

# 🏗️ Diretrizes Técnicas

Ana organizou uma série de reuniões com os times de Arquitetura, Segurança da Informação, Infraestrutura, Qualidade e Crédito para alinhar os padrões e exigências técnicas da empresa.

---

## 🧠 Time de Arquitetura

Todos os microsserviços devem seguir os seguintes padrões:

- Utilização de **DDD (Domain-Driven Design)**
- Arquitetura baseada no padrão:
  - **CQRS (Command Query Responsibility Segregation)**

---

## 🔐 Time de Segurança

As seguintes exigências de segurança foram definidas:

- Todas as APIs devem ser protegidas utilizando:
  - **Autenticação JWT (JSON Web Token)**

- Nenhum endpoint pode ser acessado sem um token válido.

- Dados sensíveis como:
  - CPF
  - Número da conta

não podem:

- transitar entre microsserviços
- ser armazenados fora do microsserviço de Usuário

---

## 🧪 Time de Qualidade

- Todas as APIs devem possuir:
  - Projeto de testes automatizados

---

## ☁️ Time de Infraestrutura

### Ambiente de Produção

O sistema deverá:

- Executar em ambiente de nuvem
- Utilizar:
  - **Kubernetes** para orquestração

### Escalabilidade

Cada microsserviço deve executar com:

- Pelo menos **2 réplicas (instâncias)**

---

## 💳 Time de Crédito

O sistema precisa ser resiliente a falhas de comunicação.

### Requisito de Idempotência

O aplicativo cliente pode:

- perder a conexão antes de receber a resposta da API
- reenviar a mesma requisição várias vezes

Por isso:

- todos os serviços devem ser **idempotentes**

---

## ⭐ Funcionalidade Opcional

Seria interessante implementar:

- API de Tarifas

no sistema inicial.

# 🏦 API Conta Corrente

---

# ✅ Cadastro de Conta Corrente

O cadastro de conta corrente deve:

- Receber:
  - CPF do usuário
  - Senha

- Validar:
  - CPF informado

- Persistir:
  - Registro na tabela `CONTACORRENTE`

- Retornar:
  - Número da conta corrente criada

---

## ❌ Regras de Erro

### CPF inválido

Retornar:

- HTTP `400 Bad Request`

Body:

```json
{
  "message": "CPF inválido",
  "type": "INVALID_DOCUMENT"
}
```

# 🛠️ Stack Tecnológica

| Tecnologia | Finalidade |
|---|---|
| .NET 10 | Plataforma principal |
| ASP.NET Core | APIs REST |
| MediatR | Implementação de CQRS |
| Dapper | Micro ORM |
| SQLite | Persistência de dados |
| Confluent.Kafka | Integração Kafka |
| JWT Bearer | Autenticação |
| Swagger | Documentação |
| Docker | Containers |
| Kubernetes | Orquestração |
| Apache Kafka | Mensageria |

---

# 📌 Padrões e Boas Práticas

- ✅ DDD
- ✅ CQRS
- ✅ SOLID
- ✅ Clean Architecture
- ✅ Microsserviços
- ✅ Event-Driven Architecture
- ✅ Escalabilidade Horizontal
- ✅ APIs RESTful
---

## Arquitetura

![Arquitetura](https://raw.githubusercontent.com/MarcosMj2rs/Pessoal/master/BankMore/Arquitetura.png)

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

## 📚 Observações

Este projeto possui finalidade:

- educacional
- técnica
- demonstrativa

e representa uma arquitetura inspirada em cenários reais de plataformas bancárias digitais.

---

<p align="center">
  Desenvolvido com dedicação por <strong>Marcos J.</strong>
</p>

---