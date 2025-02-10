FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RPS.UI/RPS.UI.csproj", "RPS.UI/"]
COPY ["RPS.Core/RPS.Core.csproj", "RPS.Core/"]
COPY ["RPS.Domain/RPS.Domain.csproj", "RPS.Domain/"]
COPY ["RPS.MediatR/RPS.MediatR.csproj", "RPS.MediatR/"]
COPY ["RPS.DAL/RPS.DAL.csproj", "RPS.DAL/"]
RUN dotnet restore "RPS.UI/RPS.UI.csproj"
COPY . .
WORKDIR "/src/RPS.UI"
RUN dotnet build "RPS.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RPS.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPS.UI.dll"]
