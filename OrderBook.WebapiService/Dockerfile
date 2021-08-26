#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["OrderBook.WebapiService/OrderBook.WebapiService.csproj", "OrderBook.WebapiService/"]
RUN dotnet restore "OrderBook.WebapiService/OrderBook.WebapiService.csproj"
COPY . .
WORKDIR "/src/OrderBook.WebapiService"
RUN dotnet build "OrderBook.WebapiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderBook.WebapiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderBook.WebapiService.dll"]