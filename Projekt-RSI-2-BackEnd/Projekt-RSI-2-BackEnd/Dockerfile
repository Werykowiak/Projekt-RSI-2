# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Projekt-RSI-2-BackEnd/Projekt-RSI-2-BackEnd.csproj", "Projekt-RSI-2-BackEnd/"]
RUN dotnet restore "Projekt-RSI-2-BackEnd/Projekt-RSI-2-BackEnd.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Projekt-RSI-2-BackEnd"
RUN dotnet build "Projekt-RSI-2-BackEnd.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Projekt-RSI-2-BackEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Projekt-RSI-2-BackEnd.dll"]
