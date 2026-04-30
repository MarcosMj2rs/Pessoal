FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

COPY . .

RUN dotnet restore src/Services/Transferencia/Transferencia.API/Transferencia.API.csproj

RUN dotnet publish src/Services/Transferencia/Transferencia.API/Transferencia.API.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Transferencia.API.dll"]