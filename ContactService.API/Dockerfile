# ── Aşama 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece .csproj dosyalarını kopyala — bağımlılık katmanını önbelleğe al
COPY ["ContactService.API/ContactService.API.csproj",           "ContactService.API/"]
COPY ["ContactService.Application/ContactService.Application.csproj", "ContactService.Application/"]
COPY ["ContactService.Domain/ContactService.Domain.csproj",     "ContactService.Domain/"]
COPY ["ContactService.Infrastructure/ContactService.Infrastructure.csproj", "ContactService.Infrastructure/"]
COPY ["Shared.Messages/Shared.Messages.csproj",                 "Shared.Messages/"]

RUN dotnet restore "ContactService.API/ContactService.API.csproj"

# Kaynak kodun tamamını kopyala ve yayınla
COPY . .
RUN dotnet publish "ContactService.API/ContactService.API.csproj" \
    -c Release -o /app/publish --no-restore

# ── Aşama 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# .NET 8 ASP.NET container varsayılan portu
EXPOSE 8080

ENTRYPOINT ["dotnet", "ContactService.API.dll"]
