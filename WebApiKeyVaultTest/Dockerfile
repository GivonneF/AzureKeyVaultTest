FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApiKeyVaultTest/WebApiKeyVaultTest.csproj", "WebApiKeyVaultTest/"]
RUN dotnet restore "WebApiKeyVaultTest/WebApiKeyVaultTest.csproj"
COPY . .
WORKDIR "/src/WebApiKeyVaultTest"
RUN dotnet build "WebApiKeyVaultTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiKeyVaultTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiKeyVaultTest.dll"]
