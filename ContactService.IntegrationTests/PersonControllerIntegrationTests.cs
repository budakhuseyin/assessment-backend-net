using ContactService.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace ContactService.IntegrationTests;

/// <summary>
/// PersonController için uçtan uca (end-to-end) integration testleri.
/// Gerçek HTTP istekleri gönderir, controller → service → InMemory DB akışını doğrular.
/// </summary>
public class PersonControllerIntegrationTests : IClassFixture<ContactServiceWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PersonControllerIntegrationTests(ContactServiceWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_BosDatabasede_BosListe_Doner()
    {
        // Act
        var response = await _client.GetAsync("/api/Person");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var persons = await response.Content.ReadFromJsonAsync<List<PersonResponse>>();
        Assert.NotNull(persons);
        Assert.Empty(persons);
    }

    [Fact]
    public async Task Create_GecerliIstek_201Created_Doner()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Company = "Test Şirketi"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Person", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<PersonResponse>();
        Assert.NotNull(created);
        Assert.Equal("Ahmet", created.FirstName);
        Assert.Equal("Yılmaz", created.LastName);
        Assert.NotEqual(Guid.Empty, created.UUID);
    }

    [Fact]
    public async Task Create_SonraGetAll_OlusturulanKisiyiListeler()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "Zeynep",
            LastName = "Kaya",
            Company = "Aras Kargo"
        };

        // Act
        await _client.PostAsJsonAsync("/api/Person", request);
        var listResponse = await _client.GetAsync("/api/Person");

        // Assert
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var persons = await listResponse.Content.ReadFromJsonAsync<List<PersonResponse>>();
        Assert.NotNull(persons);
        Assert.Contains(persons, p => p.FirstName == "Zeynep" && p.LastName == "Kaya");
    }

    [Fact]
    public async Task GetById_MevcutId_200OK_Doner()
    {
        // Arrange — Önce kişi oluştur
        var createRequest = new CreatePersonRequest
        {
            FirstName = "Mehmet",
            LastName = "Demir",
            Company = "Test A.Ş."
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Person", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        // Act
        var response = await _client.GetAsync($"/api/Person/{created!.UUID}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var person = await response.Content.ReadFromJsonAsync<PersonResponse>();
        Assert.NotNull(person);
        Assert.Equal("Mehmet", person.FirstName);
    }

    [Fact]
    public async Task GetById_YanlisId_404NotFound_Doner()
    {
        // Act
        var response = await _client.GetAsync($"/api/Person/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MevcutKisi_204NoContent_Doner()
    {
        // Arrange — Önce kişi oluştur
        var createRequest = new CreatePersonRequest
        {
            FirstName = "Silinecek",
            LastName = "Kullanıcı",
            Company = "Test"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Person", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/Person/{created!.UUID}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_SonraGetById_404NotFound_Doner()
    {
        // Arrange — Oluştur ve sil
        var createRequest = new CreatePersonRequest
        {
            FirstName = "Geçici",
            LastName = "Kayıt",
            Company = "Test"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Person", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();
        await _client.DeleteAsync($"/api/Person/{created!.UUID}");

        // Act — Silinen kişiyi bulmaya çalış
        var getResponse = await _client.GetAsync($"/api/Person/{created.UUID}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
