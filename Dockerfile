# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["DigiturnoAML.csproj", "./"]
RUN dotnet restore "DigiturnoAML.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "DigiturnoAML.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "DigiturnoAML.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port 8080 (Render default)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DigiturnoAML.dll"]
