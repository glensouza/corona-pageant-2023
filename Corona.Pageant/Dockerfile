FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY . .
RUN dotnet restore "Corona.Pageant/Corona.Pageant.csproj"
RUN dotnet build "Corona.Pageant/Corona.Pageant.csproj" -c Release -o /app/build
RUN dotnet publish "Corona.Pageant/Corona.Pageant.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0
ARG TARGETARCH
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Corona.Pageant.dll"]
