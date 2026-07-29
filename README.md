# Aras Kargo - Contact Directory & Reporting Microservices

Bu proje, bir rehber uygulamasının (Contact Directory) ve bu rehbere bağlı asenkron raporlama süreçlerinin **Mikroservis Mimarisi** ile geliştirildiği bir değerlendirme (assessment) projesidir.

## Mimari & Teknolojiler
Proje, **Clean Architecture** prensiplerine sadık kalınarak, ölçeklenebilir ve bağımsız dağıtılabilir (deployable) iki farklı mikroservis olarak tasarlanmıştır. Servisler arası iletişim **Event-Driven (Olay Güdümlü)** mimari ile sağlanmaktadır.

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core
* **Mesaj Kuyruğu (Message Broker):** RabbitMQ
* **Event Bus:** MassTransit
* **Test:** xUnit, Moq
* **Konteynerizasyon:** Docker, Docker Compose
* **Tasarım Desenleri:** Repository Pattern, Dependency Injection

---

## Mikroservisler

### 1. ContactService (Rehber Servisi)
Kişilerin (Person) ve kişilere ait iletişim bilgilerinin (ContactInfo - Telefon, E-Posta, Konum vb.) yönetildiği servistir.
* **Sorumluluklar:** Kişi ekleme, silme, listeleme, kişi detayı görüntüleme, iletişim bilgisi ekleme/silme.
* **Olay Tüketimi (Consumer):** `ReportService` tarafından fırlatılan `ReportRequestedEvent` olayını dinler (RabbitMQ üzerinden). Olayı aldığında, veritabanındaki konum bilgilerini tarar, istatistikleri (ilgili konumdaki kişi sayısı ve telefon numarası sayısı) hesaplar ve sonucu `ReportService`'in callback API'sine iletir.

### 2. ReportService (Rapor Servisi)
Kullanıcıların lokasyon bazlı istatistiksel rapor taleplerini yöneten servistir. 
* **Sorumluluklar:** Yeni rapor talebi oluşturma, raporların statülerini (Hazırlanıyor, Tamamlandı) listeleme ve rapor detaylarını görüntüleme.
* **Olay Fırlatma (Publisher):** Yeni bir rapor talep edildiğinde, raporu veritabanına `Preparing (1)` statüsüyle kaydeder ve RabbitMQ üzerinden `ReportRequestedEvent` mesajını yayınlar. 

---

## Asenkron İletişim Senaryosu (Event-Driven)
Rapor oluşturma işlemi yoğun kaynak tüketebilecek bir iş olduğundan, **asenkron** olarak tasarlanmıştır:
1. Kullanıcı `ReportService` üzerinden **POST /api/report** isteği yapar.
2. `ReportService`, veritabanına statüsü "Preparing" olan bir kayıt atar.
3. `ReportService`, RabbitMQ üzerinden **ReportRequestedEvent** fırlatır. İsteğe anında HTTP 200 (ve UUID) döner.
4. `ContactService`, RabbitMQ'dan bu mesajı alır. Arka planda ağır hesaplamayı (konumlara göre kişi/telefon sayımı) yapar.
5. Hesaplama bittikten sonra `ContactService`, `ReportService` üzerinde bulunan webhook/callback endpoint'ine (**PUT /api/report/{uuid}/complete**) sonuçları gönderir.
6. `ReportService` raporun statüsünü "Completed" yapar ve hesaplanan detayları kaydeder.

---

## Kurulum ve Çalıştırma (Docker Compose)

Projenin tüm bağımlılıkları (PostgreSQL, RabbitMQ, Microservices) Docker ile konteynerize edilmiştir. Yerel ortamınızda sadece **Docker** ve **Docker Compose** kurulu olması yeterlidir.

1. Proje dizinine gidin:
```bash
cd ArasContactDirectory
```

2. Docker Compose ile tüm altyapıyı ve servisleri ayağa kaldırın:
```bash
docker-compose up --build -d
```

*(Not: Veritabanı (ContactDb ve ReportDb) başlangıçta otomatik oluşturulacak ve Entity Framework Core Code-First Migration'ları container ayağa kalkarken otomatik uygulanacaktır.)*

### Erişim Adresleri (Docker ile)
* **RabbitMQ Management UI:** http://localhost:15672 (guest / guest)
* **ContactService Swagger:** http://localhost:5202/swagger
* **ReportService Swagger:** http://localhost:5063/swagger

---

## Birim Testler (Unit Testing)
Projedeki servis katmanları, **xUnit** ve **Moq** kullanılarak izole bir şekilde test edilmiştir. (Toplam 14 test, %100 Başarı).
Testleri çalıştırmak için:
```bash
dotnet test
```