FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotenVonSchuelernFuerLehrer.WebApi/NotenVonSchuelernFuerLehrer.WebApi.csproj", "NotenVonSchuelernFuerLehrer.WebApi/"]
COPY ["NotenVonSchuelernFuerLehrer.Domain.Service/NotenVonSchuelernFuerLehrer.Domain.Service.csproj", "NotenVonSchuelernFuerLehrer.Domain.Service/"]
COPY ["NotenVonSchuelernFuerLehrer.Domain.Model/NotenVonSchuelernFuerLehrer.Domain.Model.csproj", "NotenVonSchuelernFuerLehrer.Domain.Model/"]
RUN dotnet restore "NotenVonSchuelernFuerLehrer.WebApi/NotenVonSchuelernFuerLehrer.WebApi.csproj"
COPY . .
WORKDIR "/src/NotenVonSchuelernFuerLehrer.WebApi"
RUN dotnet build "./NotenVonSchuelernFuerLehrer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NotenVonSchuelernFuerLehrer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotenVonSchuelernFuerLehrer.WebApi.dll"]
