FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /app

COPY . ./
RUN dotnet restore

RUN dotnet test -c Release
RUN dotnet publish -c Release -o /app/out

ENTRYPOINT ["/bin/bash"]