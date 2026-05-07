Conta: 1
{
  "nome": "Alan dos Santos",
  "cpf": "91851643052",
  "senha": "1"
}

Conta: 2
{
  "nome": "Geraldo Rossi",
  "cpf": "95185068094",
  "senha": "1"
}

Conta: 3
{
  "nome": "Clarice Lispector",
  "cpf": "79542167059",
  "senha": "1"
}

=================================DOCKER===================================
......................CONTA CORRENTE / TRANSFERENCIA......................
==========================================================================

1. Build das imagens (local)

docker build --no-cache -t mj2rsdockerpass/conta-corrente:v1 -f docker/conta-corrente/Dockerfile .
docker build --no-cache -t mj2rsdockerpass/transferencia:v1 -f docker/transferencia/Dockerfile .

2. Login no Docker Hub (uma vez só)

docker login

3. Enviar imagens

docker push mj2rsdockerpass/conta-corrente:v1
docker push mj2rsdockerpass/transferencia:v1

4. Aplicar no Kubernetes

kubectl create namespace banco

kubectl apply -f .\k8s\conta-corrente-deployment.yaml -n banco
kubectl apply -f .\k8s\conta-corrente-service.yaml -n banco

kubectl apply -f .\k8s\transferencia-deployment.yaml -n banco
kubectl apply -f .\k8s\transferencia-service.yaml -n banco

*********************************************************

# 1. Rebuild forçando sem cache e com tag nova
	docker build --no-cache -t mj2rsdockerpass/transferencia:v2 -f docker/transferencia/Dockerfile .

# 2. Confirma que o appsettings está correto dentro da imagem ANTES de fazer push
	docker run --rm --entrypoint cat mj2rsdockerpass/transferencia:v2 /app/appsettings.json

# 3. Se estiver correto, faz o push
	docker push mj2rsdockerpass/transferencia:v2

# 4. Atualiza o deployment
	kubectl set image deployment/transferencia transferencia=mj2rsdockerpass/transferencia:v2 -n banco

	
=========================================================
Kafka - Subir no cluster
=========================================================

	kubectl apply -f k8s/zookeeper-deployment.yaml -n banco
	kubectl apply -f k8s/kafka-deployment.yaml -n banco
	
==========================================================================
...............................TRANSFERENCIA..............................
==========================================================================

# 1. Rebuild da imagem com tag v5
	docker build --no-cache -t mj2rsdockerpass/transferencia:v5 -f docker/transferencia/Dockerfile .

# 2. Confirma que o appsettings está correto dentro da imagem
	docker run --rm --entrypoint cat mj2rsdockerpass/transferencia:v5 /app/appsettings.json

# 3. Push para o Docker Hub
	docker push mj2rsdockerpass/transferencia:v5

# 4. Atualiza o deployment no Kubernetes
	kubectl set image deployment/transferencia transferencia=mj2rsdockerpass/transferencia:v5 -n banco

# 5. Acompanha o rollout
	kubectl rollout status deployment/transferencia -n banco

# 6. Verifica os logs
	kubectl logs -f deployment/transferencia -n banco
	
==========================================================================
..................................TARIFAS.................................
==========================================================================

# 1. Build da imagem
	docker build --no-cache -t mj2rsdockerpass/tarifas:v2 -f docker/tarifas/Dockerfile .

# 2. Confirma o appsettings dentro da imagem
	docker run --rm --entrypoint cat mj2rsdockerpass/tarifas:v2 /app/appsettings.json

# 3. Push para o Docker Hub
	docker push mj2rsdockerpass/tarifas:v2

# 4. Aplica os manifestos no Kubernetes
	kubectl apply -f k8s/tarifas-deployment.yaml -n banco
	kubectl apply -f k8s/tarifas-service.yaml -n banco

# 5. Acompanha os pods
	kubectl get pods -n banco -w

# 6. Verifica os logs
	kubectl logs -f deployment/tarifas -n banco
	
--------------------------------------------------------------------------	
***Se precisar alterar algo no código (INCREMENTE v2 ex:v3):
--------------------------------------------------------------------------	

	docker build --no-cache -t mj2rsdockerpass/tarifas:v2 -f docker/tarifas/Dockerfile .
	docker push mj2rsdockerpass/tarifas:v2
	kubectl set image deployment/tarifas tarifas=mj2rsdockerpass/tarifas:v2 -n banco
	kubectl logs -f deployment/tarifas -n banco

==========================================================================
.................Criar o tópico manualmente (mais rápido).................	
==========================================================================
	kubectl exec -it deployment/kafka -n banco -- kafka-topics --create --topic transferencias-realizadas --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
	kubectl exec -it deployment/kafka -n banco -- kafka-topics --create --topic tarifacoes-realizadas --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1


++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Etapa 4 — API Conta Corrente: Consumir Tarifações
Criar um consumidor Kafka do tópico tarifacoes-realizadas que ao receber a mensagem execute o mesmo fluxo do MovimentarContaHandler sempre debitando o valor tarifado da conta.

