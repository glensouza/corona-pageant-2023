FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS build
WORKDIR /src
COPY Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj .
RUN dotnet restore Corona.Pageant.Viewer.csproj
COPY ./Corona.Pageant.Viewer .
RUN dotnet build Corona.Pageant.Viewer.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Corona.Pageant.Viewer.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Corona.Pageant.Viewer/nginx.conf /etc/nginx/nginx.conf
