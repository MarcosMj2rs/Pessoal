FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

# Copia tudo
COPY . .

# Restaura usando o projeto correto
RUN dotnet restore src/Services/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj

# Publica
RUN dotnet publish src/Services/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj -c Release -o /out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /out .

# AQUI (copia o banco)
COPY src/Services/contacorrente.db /app/contacorrente.db

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ContaCorrente.API.dll"]