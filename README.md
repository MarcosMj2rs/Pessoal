📖 Sobre o Projeto

O BankMore é uma plataforma de banco digital construída utilizando arquitetura de microsserviços, com foco em:

Escalabilidade
Resiliência
Comunicação assíncrona
Segurança
Separação de responsabilidades
Alta coesão e baixo acoplamento

O sistema simula operações reais de uma instituição financeira moderna, incluindo:

✅ Cadastro de contas correntes
✅ Autenticação JWT
✅ Movimentações financeiras
✅ Transferências bancárias
✅ Tarifação automática assíncrona
✅ Processamento distribuído com Kafka
✅ Deploy em Kubernetes

🏗️ Arquitetura da Solução
┌─────────────────────────────────────────────────────────┐
│                        Kubernetes                       │
│                                                         │
│  ┌──────────────────┐      ┌──────────────────────────┐ │
│  │  API Conta       │◄─────│    API Transferência     │ │
│  │  Corrente        │      │                          │ │
│  │  (2 réplicas)    │      │    (2 réplicas)          │ │
│  └────────┬─────────┘      └────────────┬─────────────┘ │
│           │                             │               │
│           ▼                             ▼               │
│  ┌──────────────────┐      ┌──────────────────────────┐ │
│  │  Banco SQLite    │      │   Kafka Topic            │ │
│  │ contacorrente.db │      │ transferencias-realizadas│ │
│  └──────────────────┘      └────────────┬─────────────┘ │
│                                         │               │
│                             ┌───────────▼─────────────┐ │
│                             │      API Tarifas        │ │
│                             │   (Kafka Consumer)      │ │
│                             └───────────┬─────────────┘ │
│                                         │               │
│                             ┌───────────▼─────────────┐ │
│                             │   Kafka Topic           │ │
│                             │ tarifacoes-realizadas   │ │
│                             └───────────┬─────────────┘ │
│                                         │               │
│                          ┌──────────────▼─────────────┐ │
│                          │ API Conta Corrente         │ │
│                          │ (Consumer de Tarifação)    │ │
│                          └────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
🚀 Principais Características
🔐 Autenticação com JWT Bearer Token
🧩 Arquitetura baseada em DDD
⚡ CQRS com MediatR
📡 Comunicação assíncrona via Kafka
🐳 Containerização com Docker
☸️ Orquestração com Kubernetes
🔄 Idempotência para evitar duplicidade
📈 Escalabilidade horizontal
🧠 Consumers Kafka com BackgroundService
📄 Swagger/OpenAPI
🧪 Testes automatizados
🛠️ Stack Tecnológica
Tecnologia	Finalidade
.NET 10	Plataforma principal
ASP.NET Core	APIs REST
MediatR	Implementação de CQRS
Dapper	Micro ORM
SQLite	Persistência de dados
Confluent.Kafka	Integração Kafka
JWT Bearer	Autenticação
Swagger	Documentação
Docker	Containers
Kubernetes	Orquestração
Apache Kafka	Mensageria
Zookeeper	Coordenação Kafka
Kafka UI	Monitoramento visual
📦 Microsserviços
🔹 Conta Corrente API

Responsável por:

Cadastro de contas
Login/autenticação
Movimentações financeiras
Consulta de saldo
Consumo de eventos de tarifação
Endpoints
Método	Endpoint	Descrição
POST	/api/contacorrente	Criar conta
POST	/api/contacorrente/login	Login JWT
POST	/api/contacorrente/inativar	Inativar conta
POST	/api/contacorrente/movimentar	Crédito/Débito
GET	/api/contacorrente/saldo	Consultar saldo
Regras de Negócio
CPF validado no cadastro
Apenas contas ativas movimentam saldo
Proteção contra duplicidade
Consumo Kafka do tópico tarifacoes-realizadas
Claims JWT usando contaId
🔹 Transferência API

Responsável pelas transferências bancárias.

Endpoint
Método	Endpoint	Descrição
POST	/api/transferencia	Transferir valores
Fluxo
Debita conta origem
Credita conta destino
Registra transferência
Publica evento Kafka
Garante idempotência
🔹 Tarifas API

Microsserviço assíncrono responsável pela tarifação.

Responsabilidades
Consumir tópico transferencias-realizadas
Gerar cobrança automática
Registrar tarifa
Publicar evento tarifacoes-realizadas
🧠 Conceitos Arquiteturais Aplicados
DDD — Domain-Driven Design

Separação clara entre:

Application
Domain
Infrastructure
API

Benefícios:

Alta organização
Regras de negócio isoladas
Facilidade de manutenção
Escalabilidade da solução
CQRS — Command Query Responsibility Segregation

Separação entre:

Commands → Escrita
Queries → Leitura

Implementado utilizando:

MediatR
Comunicação Assíncrona

O sistema utiliza Apache Kafka para desacoplamento entre serviços.

Eventos principais
Evento	Responsável
transferencias-realizadas	API Transferência
tarifacoes-realizadas	API Tarifas
🔄 Fluxo Completo da Transferência
1. Cliente solicita transferência
         │
         ▼
2. API Transferência
   → Debita conta origem
   → Credita conta destino
         │
         ▼
3. Transferência registrada
         │
         ▼
4. Evento publicado no Kafka
   [transferencias-realizadas]
         │
         ▼
5. API Tarifas consome evento
         │
         ▼
6. Tarifa registrada
         │
         ▼
7. Novo evento publicado
   [tarifacoes-realizadas]
         │
         ▼
8. API Conta Corrente consome
   → Debita tarifa automaticamente
🗄️ Banco de Dados
Conta Corrente
CREATE TABLE contacorrente (
    idcontacorrente TEXT(37) PRIMARY KEY,
    numero          INTEGER NOT NULL,
    nome            TEXT NOT NULL,
    cpf             TEXT(11) NOT NULL,
    ativo           INTEGER NOT NULL,
    senha           TEXT NOT NULL,
    salt            TEXT NOT NULL
);
CREATE TABLE movimento (
    idmovimento     TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento   TEXT(25) NOT NULL,
    tipomovimento   TEXT(1) NOT NULL,
    valor           REAL NOT NULL
);
CREATE TABLE idempotencia (
    chave_idempotencia TEXT(37) PRIMARY KEY,
    requisicao TEXT NOT NULL,
    resultado TEXT
);
Transferência
CREATE TABLE transferencia (
    idtransferencia TEXT(37) PRIMARY KEY,
    idcontacorrente_origem TEXT(37) NOT NULL,
    idcontacorrente_destino TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    valor REAL NOT NULL
);
Tarifas
CREATE TABLE tarifa (
    idtarifa TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    valor REAL NOT NULL
);
📁 Estrutura do Projeto
BankMore/
│
├── docker/
├── k8s/
├── src/
│   └── Services/
│       ├── ContaCorrente/
│       ├── Transferencia/
│       └── Tarifas/
│
└── BankMore.slnx
☸️ Kubernetes

A solução roda em Kubernetes com:

Deployments
Services
Escalabilidade horizontal
Comunicação interna entre pods
Replicação de APIs
Kafka containerizado
Deploys disponíveis
conta-corrente-deployment.yaml
transferencia-deployment.yaml
tarifas-deployment.yaml
kafka-deployment.yaml
zookeeper-deployment.yaml
kafka-ui-deployment.yaml
🚀 Como Executar
Pré-requisitos
Docker Desktop
Kubernetes habilitado
.NET 10 SDK
kubectl
1. Criar namespace
kubectl create namespace banco
2. Subir infraestrutura Kafka
kubectl apply -f k8s/zookeeper-deployment.yaml -n banco
kubectl apply -f k8s/kafka-deployment.yaml -n banco
3. Criar tópicos Kafka
kubectl exec -it deployment/kafka -n banco -- \
  kafka-topics --create \
  --topic transferencias-realizadas \
  --bootstrap-server localhost:9092 \
  --partitions 1 \
  --replication-factor 1
kubectl exec -it deployment/kafka -n banco -- \
  kafka-topics --create \
  --topic tarifacoes-realizadas \
  --bootstrap-server localhost:9092 \
  --partitions 1 \
  --replication-factor 1
4. Build das imagens Docker
docker build -t seu-usuario/conta-corrente:v1 -f docker/conta-corrente/Dockerfile .
docker build -t seu-usuario/transferencia:v1 -f docker/transferencia/Dockerfile .
docker build -t seu-usuario/tarifas:v1 -f docker/tarifas/Dockerfile .
5. Push para Docker Hub
docker push seu-usuario/conta-corrente:v1
docker push seu-usuario/transferencia:v1
docker push seu-usuario/tarifas:v1
6. Deploy no Kubernetes
kubectl apply -f k8s/conta-corrente-deployment.yaml -n banco
kubectl apply -f k8s/conta-corrente-service.yaml -n banco

kubectl apply -f k8s/transferencia-deployment.yaml -n banco
kubectl apply -f k8s/transferencia-service.yaml -n banco

kubectl apply -f k8s/tarifas-deployment.yaml -n banco
kubectl apply -f k8s/tarifas-service.yaml -n banco
7. Kafka UI
kubectl apply -f k8s/kafka-ui-deployment.yaml -n banco

kubectl port-forward service/kafka-ui-service 8080:8080 -n banco

Acesse:

http://localhost:8080
🔐 Segurança
JWT Bearer Authentication
Hash + Salt nas senhas
Endpoints protegidos
Claims seguras
Comunicação desacoplada
Dados sensíveis não trafegam entre serviços
🔁 Idempotência

O projeto implementa proteção contra duplicidade de requisições utilizando:

Tabela: idempotencia

Cada requisição possui:

RequisicaoId único

Se a mesma requisição for reenviada:

✅ O sistema retorna o resultado anterior
✅ Nenhuma duplicidade financeira ocorre

📊 Observabilidade e Monitoramento
Logs
kubectl logs -f deployment/conta-corrente -n banco
kubectl logs -f deployment/transferencia -n banco
kubectl logs -f deployment/tarifas -n banco
Verificar Pods
kubectl get pods -n banco
Kafka UI
kubectl port-forward service/kafka-ui-service 8080:8080 -n banco
🧪 Testes
dotnet test src/Services/ContaCorrente/ContaCorrente.Tests/

dotnet test src/Services/Transferencia/Transferencia.Tests/
📌 Padrões e Boas Práticas
✅ DDD
✅ CQRS
✅ SOLID
✅ Clean Architecture
✅ Idempotência
✅ Microsserviços
✅ Event-Driven Architecture
✅ Background Services
✅ Escalabilidade Horizontal
✅ APIs RESTful
📚 Objetivos Técnicos do Projeto

Este projeto foi desenvolvido com foco em estudo e demonstração prática de:

Microsserviços em .NET
Kubernetes
Kafka
Arquitetura distribuída
Event-Driven Architecture
Resiliência
Segurança
Comunicação assíncrona
Escalabilidade
👨‍💻 Autor

Marcos José de Jesus

Desenvolvedor .NET especializado em arquitetura backend, microsserviços e soluções distribuídas.

⭐ Considerações Finais

O BankMore demonstra uma arquitetura moderna de sistemas financeiros baseada em:

Microsserviços
Mensageria
Containers
Orquestração
Processamento assíncrono

A solução foi construída com foco em boas práticas utilizadas em ambientes corporativos reais.
