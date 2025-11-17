FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/SmartCafe.Menu.API/SmartCafe.Menu.API.csproj", "src/SmartCafe.Menu.API/"]
COPY ["src/SmartCafe.Menu.Application/SmartCafe.Menu.Application.csproj", "src/SmartCafe.Menu.Application/"]
COPY ["src/SmartCafe.Menu.Domain/SmartCafe.Menu.Domain.csproj", "src/SmartCafe.Menu.Domain/"]
COPY ["src/SmartCafe.Menu.Infrastructure/SmartCafe.Menu.Infrastructure.csproj", "src/SmartCafe.Menu.Infrastructure/"]
RUN dotnet restore "src/SmartCafe.Menu.API/SmartCafe.Menu.API.csproj"
COPY . .
WORKDIR "/src/src/SmartCafe.Menu.API"
RUN dotnet build "SmartCafe.Menu.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCafe.Menu.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCafe.Menu.API.dll"]
