using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioBackend.Models.DTOs;
using PortfolioBackend.Services.Interfaces;

namespace PortfolioBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BlogpostReadDto>>> GetAll()
    {
        var posts = await _blogpostService.GetAllAsync();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BlogpostReadDto>> GetById(int id)
    {
        var post = await _blogpostService.GetByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<ActionResult<BlogpostReadDto>> Create(BlogpostCreateDto dto)
    {
        var createdPost = await _blogpostService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, createdPost);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<IActionResult> Update(int id, BlogpostUpdateDto dto)
    {
        var success = await _blogpostService.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Editor,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _blogpostService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}