FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Corona.Pageant.API/Corona.Pageant.API.csproj", "Corona.Pageant.API/"]
RUN dotnet restore "Corona.Pageant.API/Corona.Pageant.API.csproj"
COPY . .
WORKDIR "/src/Corona.Pageant.API"
RUN dotnet build "Corona.Pageant.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Corona.Pageant.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
