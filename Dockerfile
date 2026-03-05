FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /build

# Copia todos os projetos de uma vez para o contêiner para evitar problemas de dependência
COPY ./src/ ./

# Muda para a pasta do projeto principal da API
WORKDIR /build/CRIApp.Api

# Executa o restore para o projeto principal
RUN dotnet restore CRIApp.Api.csproj

# Publica o projeto principal em modo Release na pasta /app
RUN dotnet publish CRIApp.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV ASPNETCORE_ENVIRONMENT=Development

WORKDIR /app

COPY --from=build /app .

# Define o ponto de entrada para o contÍiner
ENTRYPOINT ["dotnet", "CRIApp.Api.dll"]
