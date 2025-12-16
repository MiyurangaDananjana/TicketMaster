FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["TicketMaster.csproj", "./"]
RUN dotnet restore "TicketMaster.csproj"

# Copy everything else
COPY . .

# Build
RUN dotnet build "TicketMaster.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicketMaster.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketMaster.dll"]