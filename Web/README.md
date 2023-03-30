# Corona Pageant Web

[![Web Docker Image CI](https://github.com/glensouza/corona-pageant-2023/actions/workflows/web-docker-publish.yml/badge.svg)](https://github.com/glensouza/corona-pageant-2023/actions/workflows/web-docker-publish.yml)

## Build the Docker Container

Run the following command:

```bash
docker build -t Corona.Pageant.Web:v1 .
```

You can confirm that this has worked by running the command:

```bash
docker images
```

## Run the Docker Container

Run the following command to run the HTML container server:

docker run -d -p 80:80 Corona.Pageant.Web:v1

## Ensure the server is running

<http://localhost>

## Run Locally on Windows with Docker Desktop

```bash
docker run -d --name CoronaPageantLocalDev -p 8080:80 -v  /$(“pwd”)/Web:/usr/share/nginx/html:ro -v  /$(“pwd”)/Web/nginx.conf:/etc/nginx/nginx.conf -v  /$(“pwd”)/Web/default.conf:/etc/nginx/conf.d/default.conf nginx:alpine
```
