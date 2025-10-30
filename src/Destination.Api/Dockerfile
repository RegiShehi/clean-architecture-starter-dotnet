FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["src/Destination.Api/Destination.Api.csproj", "src/Destination.Api/"]
COPY ["src/Destination.Application/Destination.Application.csproj", "src/Destination.Application/"]
COPY ["src/Destination.Domain/Destination.Domain.csproj", "src/Destination.Domain/"]
COPY ["src/Destination.SharedKernel/Destination.SharedKernel.csproj", "src/Destination.SharedKernel/"]
COPY ["src/Destination.Infrastructure/Destination.Infrastructure.csproj", "src/Destination.Infrastructure/"]

RUN dotnet restore "src/Destination.Api/Destination.Api.csproj"

COPY . .

WORKDIR "/src/src/Destination.Api"
RUN dotnet build "./Destination.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Destination.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Destination.Api.dll"]
