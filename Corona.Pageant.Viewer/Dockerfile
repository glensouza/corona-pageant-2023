FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY . .
RUN dotnet restore "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj"
RUN dotnet build "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj" -c Release -o /app/build
RUN dotnet publish "Corona.Pageant.Viewer/Corona.Pageant.Viewer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:stable
EXPOSE 80
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY Corona.Pageant.Viewer/nginx.conf /etc/nginx/nginx.conf
