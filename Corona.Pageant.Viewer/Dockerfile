FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj"
RUN dotnet build "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj" -c Release -o /app/build
RUN dotnet publish "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY Corona.Pageant.Viewer/nginx.conf /etc/nginx/nginx.conf
