FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Transferencia/Transferencia.API/Transferencia.API.csproj", "src/Services/Transferencia/Transferencia.API/"]
COPY ["src/Services/Transferencia/Transferencia.Application/Transferencia.Application.csproj", "src/Services/Transferencia/Transferencia.Application/"]
COPY ["src/Services/Transferencia/Transferencia.Infrastructure/Transferencia.Infrastructure.csproj", "src/Services/Transferencia/Transferencia.Infrastructure/"]
RUN dotnet restore "src/Services/Transferencia/Transferencia.API/Transferencia.API.csproj"
COPY . .
RUN dotnet publish "src/Services/Transferencia/Transferencia.API/Transferencia.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Transferencia.API.dll"]