using System.Net;
using System.Net.Http.Json;
using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Tests;

public class BlogpostsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BlogpostsApiTests(CustomWebApplicationFactory factory)
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
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<BlogpostReadDto>>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateCookieClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateCookieClient();
        var dto = new BlogpostCreateDto
        {
            Titel = "Anonieme Post",
            Inhoud = "Mag niet",
            Publicatiedatum = DateTime.UtcNow
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsGast_ReturnsForbidden()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("gast@test.local", "Test123!");
        var dto = new BlogpostUpdateDto
        {
            Titel = "Gast Update",
            Inhoud = "Mag niet",
            Publicatiedatum = DateTime.UtcNow
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidInput_AsAdmin_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("admin@test.local", "Test123!");
        var invalidDto = new BlogpostCreateDto
        {
            Titel = "", // Ongeldig: leeg
            Inhoud = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", invalidDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetById_AsAdmin_PersistsBlogpostAndReturnsData()
    {
        // Arrange
        var adminClient = await CreateAuthenticatedClientAsync("admin@test.local", "Test123!");
        var publicClient = _factory.CreateCookieClient();
        var createDto = new BlogpostCreateDto
        {
            Titel = "Identity in ASP.NET Core",
            Inhoud = "Uitleg over rollen en cookies.",
            Publicatiedatum = DateTime.UtcNow
        };

        // Act
        var postResponse = await adminClient.PostAsJsonAsync("/api/blogposts", createDto);
        var created = await postResponse.Content.ReadFromJsonAsync<BlogpostReadDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal("Identity in ASP.NET Core", created.Titel);

        // Verifieer via GET dat de blogpost daadwerkelijk is opgeslagen
        var getResponse = await publicClient.GetAsync($"/api/blogposts/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<BlogpostReadDto>();
        Assert.Equal("Uitleg over rollen en cookies.", fetched?.Inhoud);
    }

    [Fact]
    public async Task UpdateAndDelete_AsEditor_ModifiesAndRemovesBlogpost()
    {
        // Arrange
        var editorClient = await CreateAuthenticatedClientAsync("editor@test.local", "Test123!");
        var publicClient = _factory.CreateCookieClient();

        var createResponse = await editorClient.PostAsJsonAsync("/api/blogposts", new BlogpostCreateDto
        {
            Titel = "Concept Blogpost",
            Inhoud = "Eerste versie",
            Publicatiedatum = DateTime.UtcNow
        });
        var created = await createResponse.Content.ReadFromJsonAsync<BlogpostReadDto>();
        Assert.NotNull(created);

        var updateDto = new BlogpostUpdateDto
        {
            Titel = "Gepubliceerde Blogpost",
            Inhoud = "Aangepaste inhoud door Editor",
            Publicatiedatum = DateTime.UtcNow
        };

        // Act - PUT
        var putResponse = await editorClient.PutAsJsonAsync($"/api/blogposts/{created.Id}", updateDto);

        // Assert - PUT
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        var afterPut = await publicClient.GetFromJsonAsync<BlogpostReadDto>($"/api/blogposts/{created.Id}");
        Assert.Equal("Gepubliceerde Blogpost", afterPut?.Titel);
        Assert.Equal("Aangepaste inhoud door Editor", afterPut?.Inhoud);

        // Act - DELETE
        var deleteResponse = await editorClient.DeleteAsync($"/api/blogposts/{created.Id}");

        // Assert - DELETE
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var afterDelete = await publicClient.GetAsync($"/api/blogposts/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }
}