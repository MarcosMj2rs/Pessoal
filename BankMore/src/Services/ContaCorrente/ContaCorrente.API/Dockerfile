FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj", "src/Services/ContaCorrente/ContaCorrente.API/"]
COPY ["src/Services/ContaCorrente/ContaCorrente.Application/ContaCorrente.Application.csproj", "src/Services/ContaCorrente/ContaCorrente.Application/"]
COPY ["src/Services/ContaCorrente/ContaCorrente.Infrastructure/ContaCorrente.Infrastructure.csproj", "src/Services/ContaCorrente/ContaCorrente.Infrastructure/"]
RUN dotnet restore "src/Services/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj"
COPY . .
RUN dotnet publish "src/Services/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContaCorrente.API.dll"]