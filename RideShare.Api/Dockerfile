# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RideShare.slnx .
COPY RideShare.Api/RideShare.Api.csproj RideShare.Api/
COPY RideShare.Domain/RideShare.Domain.csproj RideShare.Domain/
COPY RideShare.Infrastructure/RideShare.Infrastructure.csproj RideShare.Infrastructure/
COPY RideShare.Tests/RideShare.Tests.csproj RideShare.Tests/

RUN dotnet restore RideShare.Api/RideShare.Api.csproj

COPY . .

RUN dotnet publish RideShare.Api/RideShare.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "RideShare.Api.dll"]