*[English](README.en.md) ∙ Türkçe*

# Aras Dijital - Contact Directory & Reporting Microservices

Bu proje, bir rehber uygulamasının (Contact Directory) ve bu rehbere bağlı asenkron raporlama süreçlerinin **Mikroservis Mimarisi** ile geliştirildiği, üretime hazır (production-ready) bir yazılım projesidir.

## Mimari & Teknolojiler
Proje, **Clean Architecture** prensiplerine sadık kalınarak, gevşek bağlı (loosely coupled) ve bağımsız ölçeklendirilebilir iki farklı mikroservis olarak tasarlanmıştır. Servisler arası iletişim **Event-Driven (Olay Güdümlü)** mimari ile sağlanmaktadır.

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core 8
* **Mesaj Kuyruğu (Message Broker):** RabbitMQ
* **Event Bus:** MassTransit
* **Önbellek & Hız Sınırlandırma:** Redis
* **Loglama:** Serilog (Yapısal JSON loglama)
* **Test:** xUnit, Moq, WebApplicationFactory
* **CI/CD:** GitHub Actions
* **Konteynerizasyon:** Docker, Docker Compose
* **Tasarım Desenleri:** Repository Pattern, Dependency Injection, CQRS-benzeri ayrım, Cache-Aside Pattern

---

## Mikroservisler

### 1. ContactService (Rehber Servisi)
Kişilerin (Person) ve kişilere ait iletişim bilgilerinin (ContactInfo - Telefon, E-Posta, Konum vb.) yönetildiği servistir.
* **Sorumluluklar:** Kişi ekleme, silme, listeleme, kişi detayı görüntüleme, iletişim bilgisi ekleme/silme.
* **Önbellekleme:** Sık erişilen veriler (ör. kişi detayları) Redis üzerinde tutulur.
* **Olay Tüketimi (Consumer):** `ReportService` tarafından fırlatılan `ReportRequestedEvent` olayını dinler (RabbitMQ üzerinden). Olayı aldığında, veritabanındaki konum bilgilerini tarar, istatistikleri hesaplar ve sonucu `ReportService`'e iletir.

### 2. ReportService (Rapor Servisi)
Kullanıcıların lokasyon bazlı istatistiksel rapor taleplerini yöneten servistir. 
* **Sorumluluklar:** Yeni rapor talebi oluşturma, raporların statülerini (Hazırlanıyor, Tamamlandı) listeleme ve rapor detaylarını görüntüleme.
* **Olay Fırlatma (Publisher):** Yeni bir rapor talep edildiğinde, raporu veritabanına `Preparing` statüsüyle kaydeder ve RabbitMQ üzerinden `ReportRequestedEvent` mesajını yayınlar. 

---

## Asenkron İletişim Senaryosu (Event-Driven)
Rapor oluşturma işlemi yoğun kaynak tüketebilecek bir iş olduğundan, **asenkron** olarak tasarlanmıştır:
1. Kullanıcı `ReportService` üzerinden **POST /api/report** isteği yapar.
2. `ReportService`, veritabanına statüsü "Preparing" olan bir kayıt atar.
3. `ReportService`, RabbitMQ üzerinden **ReportRequestedEvent** fırlatır. İsteğe anında UUID ile yanıt döner.
4. `ContactService`, RabbitMQ'dan bu mesajı alır. Arka planda lokasyon bazlı kişi/telefon sayımı yapar.
5. Hesaplama bittikten sonra `ContactService`, `ReportService` üzerinde bulunan API'ye (**PUT /api/report/{uuid}/complete**) sonuçları gönderir.
6. `ReportService` raporun statüsünü "Completed" yapar ve hesaplanan detayları kaydeder.

---

## Performans & Gözlemlenebilirlik (Production Readiness)

* **Global Exception Handling:** Tüm API hataları tek merkezden yakalanıp standart bir JSON formatında istemciye iletilir.
* **Rate Limiting:** Sistem kaynaklarını korumak (API yük kontrolü) amacıyla istemci başına istek limiti uygulanır.
* **Korelasyon Takibi (CorrelationId):** Her isteğe eşsiz bir kimlik (CorrelationId) atanarak, logların mikroservisler arasında uçtan uca izlenebilmesi sağlanır.
* **Health Checks:** `/health` uç noktası üzerinden PostgreSQL, Redis ve RabbitMQ servislerinin sağlığı anlık olarak izlenir.
* **Abstraction Katmanı:** Veritabanı modelleri için `BaseEntity` ve API katmanı için `BaseApiController` sınıfları aracılığıyla kod tekrarı (DRY prensibi) engellenmiştir. Veri taşıma objeleri (DTO'lar) performans için `record` tipindedir.

---

## Kurulum ve Çalıştırma (Docker Compose)

Projenin tüm bağımlılıkları (PostgreSQL, Redis, RabbitMQ, Mikroservisler) Docker ile konteynerize edilmiştir. 

1. Proje dizinine gidin:
```bash
cd ArasContactDirectory
```

2. Docker Compose ile tüm altyapıyı ve servisleri ayağa kaldırın:
```bash
docker-compose up --build -d
```
*(Veritabanı migration'ları uygulama başlatılırken otomatik uygulanır).*

### Erişim Adresleri
* **RabbitMQ Management UI:** http://localhost:15672 (guest / guest)
* **ContactService Swagger:** http://localhost:5202/swagger
* **ReportService Swagger:** http://localhost:5063/swagger
* **Health Check (Contact):** http://localhost:5202/health
* **Health Check (Report):** http://localhost:5063/health

---

## Kalite & Test Süreci (CI/CD)

Projede toplam **15 Test** bulunmaktadır (Birim ve Entegrasyon testleri).

* **Unit Tests (Birim Testleri):** `Moq` ve `xUnit` ile servis katmanları izole olarak test edilmiştir.
* **Integration Tests (Entegrasyon Testleri):** `WebApplicationFactory` kullanılarak, gerçek HTTP istekleri üzerinden uçtan uca senaryolar doğrulanır. Test ortamında dış bağımlılıkları kırmak için **InMemory DB** ve **MassTransit InMemory Transport** kullanılmıştır.
* **CI Pipeline:** `main` veya `development` dalına yapılan her Push/PR işleminde **GitHub Actions** devreye girer. .NET derlemesi (build) ve tüm testler (unit + integration) bulut ortamında otomatik olarak koşturulur.