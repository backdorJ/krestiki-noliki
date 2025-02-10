FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TicTacToe.UI/TicTacToe.UI.csproj", "TicTacToe.UI/"]
COPY ["TicTacToe.Core/TicTacToe.Core.csproj", "TicTacToe.Core/"]
COPY ["TicTacToe.Domain/TicTacToe.Domain.csproj", "TicTacToe.Domain/"]
COPY ["TicTacToe.DAL/TicTacToe.DAL.csproj", "TicTacToe.DAL/"]
RUN dotnet restore "TicTacToe.UI/TicTacToe.UI.csproj"
COPY . .
WORKDIR "/src/TicTacToe.UI"
RUN dotnet build "TicTacToe.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TicTacToe.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.UI.dll"]
