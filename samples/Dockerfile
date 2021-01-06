FROM ft-base:2 AS build
WORKDIR /app

# RUN dockerd

COPY . ./
RUN dotnet restore

# RUN dotnet test -c Release
#RUN dotnet publish -c Release -o /app/out

ENTRYPOINT ["dotnet","test", "-c", "Release"]

#docker build --add-host=docker:192.168.1.4 -t ft-5-run-6 .
#docker run -e DOCKER_CUSTOM_HOST_IP=192.168.1.4 -e ASPNETCORE_TEST_CONTENTROOT_SANDBOX=/app/sandbox/ -v //var/run/docker.sock:/var/run/docker.sock --add-host=docker:192.168.1.4 -t ft-5-run-6