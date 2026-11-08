using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Data;
using PortfolioBackend.Models.DTOs;
using PortfolioBackend.Models.Entities;
using PortfolioBackend.Services.Interfaces;

namespace PortfolioBackend.Services;

public class BlogpostService : IBlogpostService
{
    private readonly AppDbContext _context;

    public BlogpostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogpostReadDto>> GetAllAsync()
    {
        return await _context.Blogposts
            .Select(b => new BlogpostReadDto
            {
                Id = b.Id,
                Titel = b.Titel,
                Inhoud = b.Inhoud,
                Publicatiedatum = b.Publicatiedatum
            })
            .ToListAsync();
    }

    public async Task<BlogpostReadDto?> GetByIdAsync(int id)
    {
        var blog = await _context.Blogposts.FindAsync(id);
        if (blog == null) return null;

        return new BlogpostReadDto
        {
            Id = blog.Id,
            Titel = blog.Titel,
            Inhoud = blog.Inhoud,
            Publicatiedatum = blog.Publicatiedatum
        };
    }

    public async Task<BlogpostReadDto> CreateAsync(BlogpostCreateDto dto)
    {
        var entity = new Blogpost
        {
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };

        _context.Blogposts.Add(entity);
        await _context.SaveChangesAsync();

        return new BlogpostReadDto
        {
            Id = entity.Id,
            Titel = entity.Titel,
            Inhoud = entity.Inhoud,
            Publicatiedatum = entity.Publicatiedatum
        };
    }

    public async Task<bool> UpdateAsync(int id, BlogpostUpdateDto dto)
    {
        var entity = await _context.Blogposts.FindAsync(id);
        if (entity == null) return false;

        entity.Titel = dto.Titel;
        entity.Inhoud = dto.Inhoud;
        entity.Publicatiedatum = dto.Publicatiedatum;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Blogposts.FindAsync(id);
        if (entity == null) return false;

        _context.Blogposts.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}