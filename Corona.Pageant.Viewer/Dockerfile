FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Corona.Pageant.Viewer.csproj"
RUN dotnet build "Corona.Pageant.Viewer.csproj" -c Release -o /app/build
RUN dotnet publish "Corona.Pageant.Viewer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY ["nginx.conf", "/etc/nginx/nginx.conf"]
