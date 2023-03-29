# Corona Pageant Web

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
