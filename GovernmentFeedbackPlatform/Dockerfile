# ==============================================================
# SIH1329 - Multi-Stage Dockerfile for ASP.NET Core 10 MVC
# Platform for Collecting 360-Degree Feedback on Government News
# ==============================================================

# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies separately (caches layers)
COPY ["GovernmentFeedbackPlatform/GovernmentFeedbackPlatform.csproj", "GovernmentFeedbackPlatform/"]
COPY ["GovernmentFeedbackPlatform/nuget.config", "GovernmentFeedbackPlatform/"]
RUN dotnet restore "GovernmentFeedbackPlatform/GovernmentFeedbackPlatform.csproj"

# Copy entire source tree and build release artifact
COPY . .
WORKDIR "/src/GovernmentFeedbackPlatform"
RUN dotnet publish "GovernmentFeedbackPlatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Production / Development Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Configure ASP.NET Core environment and listening port
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5189
EXPOSE 5189

# Copy published binaries from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GovernmentFeedbackPlatform.dll"]
