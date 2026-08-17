*English ∙ [Türkçe](README.md)*

# Aras Dijital - Contact Directory & Reporting Microservices

This project is a production-ready software project where a contact directory application and its associated asynchronous reporting processes are developed using a **Microservices Architecture**.

## Architecture & Technologies
The project is designed as two different loosely coupled and independently scalable microservices, strictly adhering to **Clean Architecture** principles. Inter-service communication is provided by an **Event-Driven** architecture.

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core 8
* **Message Broker:** RabbitMQ
* **Event Bus:** MassTransit
* **Cache & Rate Limiting:** Redis
* **Logging:** Serilog (Structured JSON logging)
* **Testing:** xUnit, Moq, WebApplicationFactory
* **CI/CD:** GitHub Actions
* **Containerization:** Docker, Docker Compose
* **Design Patterns:** Repository Pattern, Dependency Injection, CQRS-like separation, Cache-Aside Pattern

---

## Microservices

### 1. ContactService
The service where persons and their contact information (Phone, E-Mail, Location, etc.) are managed.
* **Responsibilities:** Add, delete, list persons, view person details, add/delete contact info.
* **Caching:** Frequently accessed data (e.g., person details) are stored on Redis.
* **Event Consumption (Consumer):** Listens to the `ReportRequestedEvent` triggered by `ReportService` (via RabbitMQ). Upon receiving the event, it scans the location data in the database, calculates the statistics, and forwards the result to `ReportService`.

### 2. ReportService
The service that manages users' location-based statistical report requests.
* **Responsibilities:** Create a new report request, list report statuses (Preparing, Completed), and view report details.
* **Event Publishing (Publisher):** When a new report is requested, it saves the report to the database with a `Preparing` status and publishes the `ReportRequestedEvent` message via RabbitMQ.

---

## Asynchronous Communication Scenario (Event-Driven)
Since the report generation process can be resource-intensive, it is designed **asynchronously**:
1. The user makes a **POST /api/report** request via `ReportService`.
2. `ReportService` inserts a record with the status "Preparing" into the database.
3. `ReportService` publishes a **ReportRequestedEvent** via RabbitMQ. It returns an immediate response to the request with a UUID.
4. `ContactService` receives this message from RabbitMQ. It performs location-based person/phone counting in the background.
5. After the calculation is finished, `ContactService` sends the results to the API (**PUT /api/report/{uuid}/complete**) located on `ReportService`.
6. `ReportService` sets the report status to "Completed" and saves the calculated details.

---

## Performance & Observability (Production Readiness)

* **Global Exception Handling:** All API errors are caught from a single center and forwarded to the client in a standard JSON format.
* **Rate Limiting:** A request limit per client is applied to protect system resources (API load control).
* **Correlation Tracking (CorrelationId):** By assigning a unique ID (CorrelationId) to each request, logs can be traced end-to-end between microservices.
* **Health Checks:** The health of PostgreSQL, Redis, and RabbitMQ services is monitored instantly via the `/health` endpoint.
* **Abstraction Layer:** Code duplication (DRY principle) is prevented through `BaseEntity` for database models and `BaseApiController` classes for the API layer. Data transfer objects (DTOs) are of `record` type for performance.

---

## Setup and Running (Docker Compose)

All dependencies of the project (PostgreSQL, Redis, RabbitMQ, Microservices) are containerized with Docker.

1. Go to the project directory:
```bash
cd ArasContactDirectory
```

2. Stand up all infrastructure and services with Docker Compose:
```bash
docker-compose up --build -d
```
*(Database migrations are automatically applied when the application starts).*

### Access Addresses
* **RabbitMQ Management UI:** http://localhost:15672 (guest / guest)
* **ContactService Swagger:** http://localhost:5202/swagger
* **ReportService Swagger:** http://localhost:5063/swagger
* **Health Check (Contact):** http://localhost:5202/health
* **Health Check (Report):** http://localhost:5063/health

---

## Quality & Testing Process (CI/CD)

There are a total of **15 Tests** in the project (Unit and Integration tests).

* **Unit Tests:** Service layers were tested in isolation with `Moq` and `xUnit`.
* **Integration Tests:** End-to-end scenarios are verified via real HTTP requests using `WebApplicationFactory`. **InMemory DB** and **MassTransit InMemory Transport** were used to break external dependencies in the test environment.
* **CI Pipeline:** **GitHub Actions** is activated on every Push/PR operation to the `main` or `development` branch. .NET build and all tests (unit + integration) are run automatically in the cloud environment.
