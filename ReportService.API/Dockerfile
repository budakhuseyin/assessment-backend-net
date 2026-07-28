# ── Aşama 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece .csproj dosyalarını kopyala — bağımlılık katmanını önbelleğe al
COPY ["ReportService.API/ReportService.API.csproj",                         "ReportService.API/"]
COPY ["ReportService.Application/ReportService.Application.csproj",         "ReportService.Application/"]
COPY ["ReportService.Domain/ReportService.Domain.csproj",                   "ReportService.Domain/"]
COPY ["ReportService.Infrastructure/ReportService.Infrastructure.csproj",   "ReportService.Infrastructure/"]
COPY ["Shared.Messages/Shared.Messages.csproj",                             "Shared.Messages/"]

RUN dotnet restore "ReportService.API/ReportService.API.csproj"

# Kaynak kodun tamamını kopyala ve yayınla
COPY . .
RUN dotnet publish "ReportService.API/ReportService.API.csproj" \
    -c Release -o /app/publish --no-restore

# ── Aşama 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# .NET 8 ASP.NET container varsayılan portu
EXPOSE 8080

ENTRYPOINT ["dotnet", "ReportService.API.dll"]
