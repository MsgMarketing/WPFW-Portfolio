using System.ComponentModel.DataAnnotations;

namespace PortfolioBackend.Models.DTOs;

// Uitvoer-DTO voor GET (bevat Id)
public class BlogpostReadDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}

// Invoer-DTO voor POST (geen Id)
public class BlogpostCreateDto
{
    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(200, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inhoud is verplicht.")]
    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}

// Invoer-DTO voor PUT
public class BlogpostUpdateDto
{
    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(200, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inhoud is verplicht.")]
    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}