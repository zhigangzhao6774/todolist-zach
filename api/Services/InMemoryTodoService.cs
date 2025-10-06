using Api.Models;
using System.Collections.Concurrent;

namespace Api.Services;

/// <summary>
/// In-memory implementation of the TODO service
/// Thread-safe using ConcurrentDictionary
/// </summary>
public class InMemoryTodoService : ITodoService
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _todoItems = new();

    public InMemoryTodoService()
    {
        // Seed with some sample data for demonstration
        SeedData();
    }

    public Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        var items = _todoItems.Values.OrderBy(x => x.CreatedAt);
        return Task.FromResult<IEnumerable<TodoItem>>(items);
    }

    public Task<TodoItem?> GetByIdAsync(Guid id)
    {
        _todoItems.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<TodoItem> CreateAsync(CreateTodoItemDto createDto)
    {
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            IsCompleted = createDto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        _todoItems.TryAdd(todoItem.Id, todoItem);
        return Task.FromResult(todoItem);
    }

    public Task<TodoItem?> UpdateAsync(Guid id, UpdateTodoItemDto updateDto)
    {
        if (!_todoItems.TryGetValue(id, out var existingItem))
        {
            return Task.FromResult<TodoItem?>(null);
        }

        if (updateDto.Title != null)
        {
            existingItem.Title = updateDto.Title;
        }

        if (updateDto.IsCompleted.HasValue)
        {
            existingItem.IsCompleted = updateDto.IsCompleted.Value;
        }

        existingItem.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<TodoItem?>(existingItem);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_todoItems.TryRemove(id, out _));
    }

    private void SeedData()
    {
        var sampleItems = new[]
        {
            new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Complete the TODO list application",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Write comprehensive tests",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Create README documentation",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var item in sampleItems)
        {
            _todoItems.TryAdd(item.Id, item);
        }
    }
}
