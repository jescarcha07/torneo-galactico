FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/TorneoGalactico.Domain/TorneoGalactico.Domain.csproj                 src/TorneoGalactico.Domain/
COPY src/TorneoGalactico.Application/TorneoGalactico.Application.csproj       src/TorneoGalactico.Application/
COPY src/TorneoGalactico.Infrastructure/TorneoGalactico.Infrastructure.csproj src/TorneoGalactico.Infrastructure/
COPY src/TorneoGalactico.Web/TorneoGalactico.Web.csproj                       src/TorneoGalactico.Web/
RUN dotnet restore src/TorneoGalactico.Web/TorneoGalactico.Web.csproj

COPY src/ src/
RUN dotnet publish src/TorneoGalactico.Web/TorneoGalactico.Web.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "TorneoGalactico.Web.dll"]
