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


# FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
# RUN apk add --no-cache \
#         ca-certificates \
# # DOCKER_HOST=ssh://... -- https://github.com/docker/cli/pull/1014
#         openssh-client
#
# # set up nsswitch.conf for Go's "netgo" implementation (which Docker explicitly uses)
# # - https://github.com/docker/docker-ce/blob/v17.09.0-ce/components/engine/hack/make.sh#L149
# # - https://github.com/golang/go/blob/go1.9.1/src/net/conf.go#L194-L275
# # - docker run --rm debian:stretch grep '^hosts:' /etc/nsswitch.conf
# RUN [ ! -e /etc/nsswitch.conf ] && echo 'hosts: files dns' > /etc/nsswitch.conf
#
# ENV DOCKER_CHANNEL stable
# ENV DOCKER_VERSION 19.03.12
# # TODO ENV DOCKER_SHA256
# # https://github.com/docker/docker-ce/blob/5b073ee2cf564edee5adca05eee574142f7627bb/components/packaging/static/hash_files !!
# # (no SHA file artifacts on download.docker.com yet as of 2017-06-07 though)
#
# RUN set -eux; \
#     \
# # this "case" statement is generated via "update.sh"
#     apkArch="$(apk --print-arch)"; \
#     case "$apkArch" in \
# # amd64
#         x86_64) dockerArch='x86_64' ;; \
# # arm32v6
#         armhf) dockerArch='armel' ;; \
# # arm32v7
#         armv7) dockerArch='armhf' ;; \
# # arm64v8
#         aarch64) dockerArch='aarch64' ;; \
#         *) echo >&2 "error: unsupported architecture ($apkArch)"; exit 1 ;;\
#     esac; \
#     \
#     if ! wget -O docker.tgz "https://download.docker.com/linux/static/${DOCKER_CHANNEL}/${dockerArch}/docker-${DOCKER_VERSION}.tgz"; then \
#         echo >&2 "error: failed to download 'docker-${DOCKER_VERSION}' from '${DOCKER_CHANNEL}' for '${dockerArch}'"; \
#         exit 1; \
#     fi; \
#     \
#     tar --extract \
#         --file docker.tgz \
#         --strip-components 1 \
#         --directory /usr/local/bin/ \
#     ; \
#     rm docker.tgz; \
#     \
#     dockerd --version; \
#     docker --version
#
# COPY modprobe.sh /usr/local/bin/modprobe
# COPY docker-entrypoint.sh /usr/local/bin/
#
# # https://github.com/docker-library/docker/pull/166
# #   dockerd-entrypoint.sh uses DOCKER_TLS_CERTDIR for auto-generating TLS certificates
# #   docker-entrypoint.sh uses DOCKER_TLS_CERTDIR for auto-setting DOCKER_TLS_VERIFY and DOCKER_CERT_PATH
# # (For this to work, at least the "client" subdirectory of this path needs to be shared between the client and server containers via a volume, "docker cp", or other means of data sharing.)
# ENV DOCKER_TLS_CERTDIR=/certs
# # also, ensure the directory pre-exists and has wide enough permissions for "dockerd-entrypoint.sh" to create subdirectories, even when run in "rootless" mode
# RUN mkdir /certs /certs/client && chmod 1777 /certs /certs/client
# # (doing both /certs and /certs/client so that if Docker does a "copy-up" into a volume defined on /certs/client, it will "do the right thing" by default in a way that still works for rootless users)
#
## ENV COMPOSE_VERSION 1.26.2
##
## RUN apk add --no-cache py3-pip python3
## RUN apk add --no-cache --virtual build-dependencies python3-dev libffi-dev openssl-dev gcc libc-dev make \
##   && pip3 install "docker-compose${COMPOSE_VERSION:+==}${COMPOSE_VERSION}" \
##   && apk del build-dependencies
#
# ENTRYPOINT ["docker-entrypoint.sh"]
# CMD ["sh"]