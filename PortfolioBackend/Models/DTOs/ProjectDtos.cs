using System.ComponentModel.DataAnnotations;

namespace PortfolioBackend.Models.DTOs;

// Uitvoer-DTO voor GET (bevat Id)
public class ProjectReadDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}

// Invoer-DTO voor POST (geen Id, database genereert dit)
public class ProjectCreateDto
{
    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(200, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beschrijving is verplicht.")]
    public string Beschrijving { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categorie is verplicht.")]
    public string Categorie { get; set; } = string.Empty;

    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}

// Invoer-DTO voor PUT (geen Id in de body; Id komt via de URL)
public class ProjectUpdateDto
{
    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(200, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beschrijving is verplicht.")]
    public string Beschrijving { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categorie is verplicht.")]
    public string Categorie { get; set; } = string.Empty;

    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}