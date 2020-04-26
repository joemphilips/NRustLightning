FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["src/NRustLightning.Server/NRustLightning.Server.csproj", "src/NRustLightning.Server/"]
RUN dotnet restore "src/NRustLightning.Server/NRustLightning.Server.csproj"
COPY . .
WORKDIR "/src/src/NRustLightning.Server"
RUN dotnet build "NRustLightning.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NRustLightning.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NRustLightning.Server.dll"]
