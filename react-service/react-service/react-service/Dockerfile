# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["react-service/react-service.csproj", "react-service/"]
COPY ["react-service.Application/react-service.Application.csproj", "react-service.Application/"]
COPY ["react-service.Domain/react-service.Domain.csproj", "react-service.Domain/"]
COPY ["react-service.Infrastructure/react-service.Infrastructure.csproj", "react-service.Infrastructure/"]
COPY ["Worker/Worker.csproj", "Worker/"]
RUN dotnet restore "./react-service/react-service.csproj"
COPY . .
WORKDIR "/src/react-service"
RUN dotnet build "./react-service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./react-service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "react-service.dll"]