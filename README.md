### AdayYonetimPlatformu

Backend: .NET 8 Web API + MongoDB. Docker Compose ile çalıştırılır.

#### Gereksinimler
- Docker ve Docker Compose
- Alternatif: .NET SDK 8.0 (Docker kullanmayacaksanız)

#### Hızlı Başlangıç (Docker ile önerilen)
1) Örnek ortam değişkenlerini kopyalayın ve gizli anahtarı doldurun:
```bash
cp example.env .env
# .env içindeki JwtSettings__SecretKey değerini güçlü bir anahtar ile değiştirin
```
2) Servisleri başlatın:
```bash
docker compose up -d --build
```
3) API: `http://localhost:5005` — Swagger: `http://localhost:5005/swagger`

#### Lokal Geliştirme (Docker olmadan)
- MongoDB kurun (veya sadece MongoDB konteynerini çalıştırın: `docker compose up -d mongodb`).
- Aşağıdaki ortam değişkenlerini ayarlayın veya `YetenekYonetimAPI/appsettings.json` içine uygun değerleri yazın:
```bash
export MongoDbSettings__ConnectionString="mongodb://localhost:27017"
export MongoDbSettings__DatabaseName="YetenekYonetimDB"
export JwtSettings__SecretKey="replace_with_strong_random_key"
export JwtSettings__Issuer="YetenekYonetimAPI"
export JwtSettings__Audience="YetenekYonetimAPI"
```
- Uygulamayı çalıştırın:
```bash
dotnet run --project YetenekYonetimAPI
```

#### Konfigürasyon
- `MongoDbSettings__ConnectionString`: Mongo bağlantı adresi (Docker'da varsayılan `mongodb://mongodb:27017`).
- `MongoDbSettings__DatabaseName`: Veritabanı adı.
- `JwtSettings__SecretKey`: JWT imza anahtarı (güçlü ve gizli olmalı). Örnek güçlü key üretimi:
```bash
python - <<'PY'
import secrets
print(secrets.token_hex(32))
PY
```
- `JwtSettings__Issuer`, `JwtSettings__Audience`: JWT ayarları.

#### Güvenlik ve Gizlilik
- `appsettings.json` içinde gizli değer tutulmaz. Gizli değerleri `.env` veya CI/CD secret olarak geçin.
- `.gitignore` içinde `*.env` ve lokal appsettings dosyaları ignore edilir.

#### Sorun Giderme
- Build: .NET 8 kurulu mu kontrol edin: `dotnet --info`.
- Compose doğrulaması: `docker compose config`.
- Mongo logları: `docker logs mongodb_container`.
