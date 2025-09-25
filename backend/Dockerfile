# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["JobSystem.Api/JobSystem.Api.csproj", "JobSystem.Api/"]
RUN dotnet restore "JobSystem.Api/JobSystem.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/JobSystem.Api"
RUN dotnet build "JobSystem.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "JobSystem.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Chrome for Selenium (if needed for scraping) - Skip for ARM64 compatibility
# RUN apt-get update && apt-get install -y \
#     wget \
#     gnupg \
#     && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
#     && echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list \
#     && apt-get update \
#     && apt-get install -y google-chrome-stable \
#     && rm -rf /var/lib/apt/lists/*

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "JobSystem.Api.dll"]