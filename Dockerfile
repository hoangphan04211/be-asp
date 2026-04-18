# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["QLKHO_PhanVanHoang/QLKHO_PhanVanHoang.csproj", "QLKHO_PhanVanHoang/"]
RUN dotnet restore "QLKHO_PhanVanHoang/QLKHO_PhanVanHoang.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/QLKHO_PhanVanHoang"
RUN dotnet build "QLKHO_PhanVanHoang.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "QLKHO_PhanVanHoang.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (Render defaults to 8080 for .NET 8)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "QLKHO_PhanVanHoang.dll"]
