using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for managing TODO items
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all TODO items
    /// </summary>
    /// <returns>List of all TODO items</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        _logger.LogInformation("Getting all TODO items");
        var items = await _todoService.GetAllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Get a specific TODO item by ID
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item</param>
    /// <returns>The TODO item if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItem>> GetById(Guid id)
    {
        _logger.LogInformation("Getting TODO item with ID: {Id}", id);
        var item = await _todoService.GetByIdAsync(id);

        if (item == null)
        {
            _logger.LogWarning("TODO item with ID {Id} not found", id);
            return NotFound(new { message = $"TODO item with ID {id} not found" });
        }

        return Ok(item);
    }

    /// <summary>
    /// Create a new TODO item
    /// </summary>
    /// <param name="createDto">The data for creating the TODO item</param>
    /// <returns>The created TODO item</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoItemDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new TODO item: {Title}", createDto.Title);
        var item = await _todoService.CreateAsync(createDto);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Update an existing TODO item
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item to update</param>
    /// <param name="updateDto">The data for updating the TODO item</param>
    /// <returns>The updated TODO item</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItem>> Update(Guid id, [FromBody] UpdateTodoItemDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating TODO item with ID: {Id}", id);
        var item = await _todoService.UpdateAsync(id, updateDto);

        if (item == null)
        {
            _logger.LogWarning("TODO item with ID {Id} not found", id);
            return NotFound(new { message = $"TODO item with ID {id} not found" });
        }

        return Ok(item);
    }

    /// <summary>
    /// Delete a TODO item
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting TODO item with ID: {Id}", id);
        var deleted = await _todoService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("TODO item with ID {Id} not found", id);
            return NotFound(new { message = $"TODO item with ID {id} not found" });
        }

        return NoContent();
    }
}
