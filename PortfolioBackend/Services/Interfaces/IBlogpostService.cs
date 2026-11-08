using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Services.Interfaces;

public interface IBlogpostService
{
    Task<IEnumerable<BlogpostReadDto>> GetAllAsync();
    Task<BlogpostReadDto?> GetByIdAsync(int id);
    Task<BlogpostReadDto> CreateAsync(BlogpostCreateDto dto);
    Task<bool> UpdateAsync(int id, BlogpostUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}