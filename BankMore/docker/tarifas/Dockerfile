FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

COPY . .

RUN dotnet restore src/Services/Tarifas/Tarifas.API/Tarifas.API.csproj

RUN dotnet publish src/Services/Tarifas/Tarifas.API/Tarifas.API.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /out .

# AQUI (copia o banco)
COPY src/Services/tarifacao.db /app/tarifacao.db


ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Tarifas.API.dll"]