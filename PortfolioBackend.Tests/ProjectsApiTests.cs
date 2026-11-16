using System.Net;
using System.Net.Http.Json;
using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Tests;

public class ProjectsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProjectsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = _factory.CreateCookieClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = email,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();
        return client;
    }

    [Fact]
    public async Task GetAll_Anonymous_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateCookieClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<ProjectReadDto>>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateCookieClient();

        // Act
        var response = await client.GetAsync("/api/projects/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateCookieClient();
        var dto = new ProjectCreateDto
        {
            Titel = "Anoniem Project",
            Beschrijving = "Mag niet lukken",
            Categorie = "Web",
            GitHubUrl = "https://github.com/test/repo",
            Datum = DateTime.UtcNow
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_AsGast_ReturnsForbidden()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("gast@test.local", "Test123!");
        var dto = new ProjectCreateDto
        {
            Titel = "Gast Project",
            Beschrijving = "Mag niet lukken",
            Categorie = "Web",
            GitHubUrl = "https://github.com/test/repo",
            Datum = DateTime.UtcNow
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidInput_AsEditor_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("editor@test.local", "Test123!");
        var invalidDto = new ProjectCreateDto
        {
            Titel = "", // Verplicht veld leeg gelaten
            Beschrijving = "",
            Categorie = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", invalidDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetById_AsEditor_PersistsProjectAndReturnsData()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("editor@test.local", "Test123!");
        var publicClient = _factory.CreateCookieClient();
        var createDto = new ProjectCreateDto
        {
            Titel = "Portfolio API",
            Beschrijving = "Beveiligd met Identity",
            Categorie = "Backend",
            GitHubUrl = "https://github.com/MsgMarketing/WPFW-Portfolio",
            Datum = DateTime.UtcNow
        };

        // Act
        var postResponse = await editorClient.PostAsJsonAsync("/api/projects", createDto);
        var created = await postResponse.Content.ReadFromJsonAsync<ProjectReadDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("Portfolio API", created.Titel);

        // Verifieer via GET dat het item daadwerkelijk in de database staat
        var getResponse = await publicClient.GetAsync($"/api/projects/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.Equal("Beveiligd met Identity", fetched?.Beschrijving);
    }

    [Fact]
    public async Task UpdateAndDelete_AsAdmin_ModifiesAndRemovesProject()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("admin@test.local", "Test123!");
        var publicClient = _factory.CreateCookieClient();

        var createResponse = await adminClient.PostAsJsonAsync("/api/projects", new ProjectCreateDto
        {
            Titel = "Origineel Project",
            Beschrijving = "Voor update",
            Categorie = "Test",
            GitHubUrl = "https://github.com/test/orig",
            Datum = DateTime.UtcNow
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.NotNull(created);

        var updateDto = new ProjectUpdateDto
        {
            Titel = "Bijgewerkt Project",
            Beschrijving = "Na update door Admin",
            Categorie = "Updated",
            GitHubUrl = "https://github.com/test/updated",
            Datum = DateTime.UtcNow
        };

        // Act - PUT
        var putResponse = await adminClient.PutAsJsonAsync($"/api/projects/{created.Id}", updateDto);

        // Assert - PUT
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        var afterPut = await publicClient.GetFromJsonAsync<ProjectReadDto>($"/api/projects/{created.Id}");
        Assert.Equal("Bijgewerkt Project", afterPut?.Titel);
        Assert.Equal("Na update door Admin", afterPut?.Beschrijving);

        // Act - DELETE
        var deleteResponse = await adminClient.DeleteAsync($"/api/projects/{created.Id}");

        // Assert - DELETE
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var afterDelete = await publicClient.GetAsync($"/api/projects/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }
}