#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FastDeliveruu.Api/FastDeliveruu.Api.csproj", "src/FastDeliveruu.Api/"]
COPY ["src/FastDeliveruu.Application/FastDeliveruu.Application.csproj", "src/FastDeliveruu.Application/"]
COPY ["src/FastDeliveruu.Domain/FastDeliveruu.Domain.csproj", "src/FastDeliveruu.Domain/"]
COPY ["src/FastDeliveruu.Infrastructure/FastDeliveruu.Infrastructure.csproj", "src/FastDeliveruu.Infrastructure/"]
RUN dotnet restore "./src/FastDeliveruu.Api/FastDeliveruu.Api.csproj"
COPY . .
WORKDIR "/src/src/FastDeliveruu.Api"
RUN dotnet build "./FastDeliveruu.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FastDeliveruu.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FastDeliveruu.Api.dll"]