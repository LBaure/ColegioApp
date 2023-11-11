FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get install -y curl jq
    
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Backend.sln ./
COPY NuGet.Config ./
COPY Core/*.csproj ./Core/
COPY Repositorios/*.csproj ./Repositorios/
COPY WebApi/*.csproj ./WebApi/

RUN dotnet restore --configfile NuGet.Config -nowarn:msb3202,nu1503 --verbosity diag

COPY . .

WORKDIR /src/WebApi
RUN dotnet build -c Release -o /app-bin

FROM build AS publish
RUN dotnet publish -c Release -o /app
RUN ls /app-bin

COPY *.xml /app
COPY WebApi/sso-qa.xml  /app
COPY WebApi/sso-prod.xml  /app

FROM base AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app .
RUN useradd -r ew_user -d /expedientes-web/home/ew_user -s /sbin/nologin;
USER ew_user
ENTRYPOINT ["dotnet", "WebApi.dll"]