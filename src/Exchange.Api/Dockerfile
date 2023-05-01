#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Exchange.Api/Exchange.Api.csproj", "src/Exchange.Api/"]
COPY ["src/Exchange.Application/Exchange.Application.csproj", "src/Exchange.Application/"]
COPY ["src/Exchange.Caching/Exchange.Caching.csproj", "src/Exchange.Caching/"]
COPY ["src/Exchange.Domain/Exchange.Domain.csproj", "src/Exchange.Domain/"]
COPY ["src/Exchange.RabbitMQBus/Exchange.RabbitMQBus.csproj", "src/Exchange.RabbitMQBus/"]
COPY ["src/Exchange.Infrastructure/Exchange.Infrastructure.csproj", "src/Exchange.Infrastructure/"]
RUN dotnet restore "src/Exchange.Api/Exchange.Api.csproj"
COPY . .
WORKDIR "/src/src/Exchange.Api"
RUN dotnet build "Exchange.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exchange.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Api.dll"]