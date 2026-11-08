using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Data;
using PortfolioBackend.Models.DTOs;
using PortfolioBackend.Models.Entities;
using PortfolioBackend.Services.Interfaces;

namespace PortfolioBackend.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectReadDto>> GetAllAsync()
    {
        return await _context.Projects
            .Select(p => new ProjectReadDto
            {
                Id = p.Id,
                Titel = p.Titel,
                Beschrijving = p.Beschrijving,
                Categorie = p.Categorie,
                GitHubUrl = p.GitHubUrl,
                Datum = p.Datum
            })
            .ToListAsync();
    }

    public async Task<ProjectReadDto?> GetByIdAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return null;

        return new ProjectReadDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl,
            Datum = project.Datum
        };
    }

    public async Task<ProjectReadDto> CreateAsync(ProjectCreateDto dto)
    {
        var entity = new Project
        {
            Titel = dto.Titel,
            Beschrijving = dto.Beschrijving,
            Categorie = dto.Categorie,
            GitHubUrl = dto.GitHubUrl,
            Datum = dto.Datum
        };

        _context.Projects.Add(entity);
        await _context.SaveChangesAsync();

        return new ProjectReadDto
        {
            Id = entity.Id,
            Titel = entity.Titel,
            Beschrijving = entity.Beschrijving,
            Categorie = entity.Categorie,
            GitHubUrl = entity.GitHubUrl,
            Datum = entity.Datum
        };
    }

    public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var entity = await _context.Projects.FindAsync(id);
        if (entity == null) return false;

        entity.Titel = dto.Titel;
        entity.Beschrijving = dto.Beschrijving;
        entity.Categorie = dto.Categorie;
        entity.GitHubUrl = dto.GitHubUrl;
        entity.Datum = dto.Datum;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Projects.FindAsync(id);
        if (entity == null) return false;

        _context.Projects.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}