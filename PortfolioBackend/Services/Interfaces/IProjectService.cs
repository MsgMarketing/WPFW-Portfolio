using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectReadDto>> GetAllAsync();
    Task<ProjectReadDto?> GetByIdAsync(int id);
    Task<ProjectReadDto> CreateAsync(ProjectCreateDto dto);
    Task<bool> UpdateAsync(int id, ProjectUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}