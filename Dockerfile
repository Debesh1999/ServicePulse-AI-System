# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy backend solution
COPY backend/src/ServicePulse.Domain/ServicePulse.Domain.csproj backend/src/ServicePulse.Domain/
COPY backend/src/ServicePulse.Application/ServicePulse.Application.csproj backend/src/ServicePulse.Application/
COPY backend/src/ServicePulse.Infrastructure/ServicePulse.Infrastructure.csproj backend/src/ServicePulse.Infrastructure/
COPY backend/src/ServicePulse.Api/ServicePulse.Api.csproj backend/src/ServicePulse.Api/
RUN dotnet restore backend/src/ServicePulse.Api/ServicePulse.Api.csproj

# Copy all source and build
COPY backend/src/ServicePulse.Domain/ backend/src/ServicePulse.Domain/
COPY backend/src/ServicePulse.Application/ backend/src/ServicePulse.Application/
COPY backend/src/ServicePulse.Infrastructure/ backend/src/ServicePulse.Infrastructure/
COPY backend/src/ServicePulse.Api/ backend/src/ServicePulse.Api/
WORKDIR "/src/backend/src/ServicePulse.Api"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copy built frontend static files
COPY backend/frontend/ServicePulse.Frontend.Vite/dist/ ./wwwroot/

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ServicePulse.Api.dll"]
