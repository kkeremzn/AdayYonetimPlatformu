# Aşama 1: Builder
# .NET SDK imajını kullanarak projenin yayın sürümünü (publish) oluşturur.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app/YetenekYonetimAPI

# Proje dosyasını kopyalar ve bağımlılıkları yükler.
# docker-compose.yml'deki context ayarına göre dosya yolu belirtilir.
COPY YetenekYonetimAPI/YetenekYonetimAPI.csproj ./
RUN dotnet restore

# Tüm proje dosyalarını kopyalar.
COPY YetenekYonetimAPI/. ./

# Projeyi yayınlar. Release modunda, çıktılar /app/publish dizinine kaydedilir.
RUN dotnet publish -c Release -o /app/publish

# Aşama 2: Final
# Daha küçük olan ASP.NET Runtime imajını kullanır.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/publish .

# Uygulamayı çalıştırır.
ENTRYPOINT ["dotnet", "YetenekYonetimAPI.dll"]