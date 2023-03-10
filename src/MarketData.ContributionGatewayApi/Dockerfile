FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MarketData.ContributionGatewayApi/MarketData.ContributionGatewayApi.csproj", "MarketData.ContributionGatewayApi/"]
RUN dotnet restore "MarketData.ContributionGatewayApi/MarketData.ContributionGatewayApi.csproj"
COPY . .
WORKDIR "/src/MarketData.ContributionGatewayApi"
RUN dotnet build "MarketData.ContributionGatewayApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarketData.ContributionGatewayApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarketData.ContributionGatewayApi.dll"]
