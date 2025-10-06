using Api.Models;

namespace Api.Services;

/// <summary>
/// Interface for TODO item service operations
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Get all TODO items
    /// </summary>
    /// <returns>Collection of all TODO items</returns>
    Task<IEnumerable<TodoItem>> GetAllAsync();

    /// <summary>
    /// Get a specific TODO item by ID
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item</param>
    /// <returns>The TODO item if found, null otherwise</returns>
    Task<TodoItem?> GetByIdAsync(Guid id);

    /// <summary>
    /// Create a new TODO item
    /// </summary>
    /// <param name="createDto">The data for creating the TODO item</param>
    /// <returns>The created TODO item</returns>
    Task<TodoItem> CreateAsync(CreateTodoItemDto createDto);

    /// <summary>
    /// Update an existing TODO item
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item to update</param>
    /// <param name="updateDto">The data for updating the TODO item</param>
    /// <returns>The updated TODO item if found, null otherwise</returns>
    Task<TodoItem?> UpdateAsync(Guid id, UpdateTodoItemDto updateDto);

    /// <summary>
    /// Delete a TODO item
    /// </summary>
    /// <param name="id">The unique identifier of the TODO item to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteAsync(Guid id);
}
