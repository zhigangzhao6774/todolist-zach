using Api.Models;
using Api.Services;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Services;

public class InMemoryTodoServiceTests
{
    private readonly InMemoryTodoService _service;

    public InMemoryTodoServiceTests()
    {
        _service = new InMemoryTodoService();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Act
        var items = await _service.GetAllAsync();

        // Assert
        items.Should().NotBeNull();
        items.Should().HaveCountGreaterThan(0); // Should have seed data
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewItem()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = "Test TODO Item",
            IsCompleted = false
        };

        // Act
        var createdItem = await _service.CreateAsync(createDto);

        // Assert
        createdItem.Should().NotBeNull();
        createdItem.Id.Should().NotBeEmpty();
        createdItem.Title.Should().Be(createDto.Title);
        createdItem.IsCompleted.Should().Be(createDto.IsCompleted);
        createdItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnItem()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "Test Item" };
        var createdItem = await _service.CreateAsync(createDto);

        // Act
        var retrievedItem = await _service.GetByIdAsync(createdItem.Id);

        // Assert
        retrievedItem.Should().NotBeNull();
        retrievedItem!.Id.Should().Be(createdItem.Id);
        retrievedItem.Title.Should().Be(createDto.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var retrievedItem = await _service.GetByIdAsync(Guid.NewGuid());

        // Assert
        retrievedItem.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdateItem()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "Original Title", IsCompleted = false };
        var createdItem = await _service.CreateAsync(createDto);

        var updateDto = new UpdateTodoItemDto
        {
            Title = "Updated Title",
            IsCompleted = true
        };

        // Act
        var updatedItem = await _service.UpdateAsync(createdItem.Id, updateDto);

        // Assert
        updatedItem.Should().NotBeNull();
        updatedItem!.Title.Should().Be(updateDto.Title);
        updatedItem.IsCompleted.Should().Be(updateDto.IsCompleted!.Value);
        updatedItem.UpdatedAt.Should().NotBeNull();
        updatedItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateTodoItemDto { Title = "Updated Title" };

        // Act
        var updatedItem = await _service.UpdateAsync(Guid.NewGuid(), updateDto);

        // Assert
        updatedItem.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithPartialUpdate_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "Original Title", IsCompleted = false };
        var createdItem = await _service.CreateAsync(createDto);

        var updateDto = new UpdateTodoItemDto { IsCompleted = true };

        // Act
        var updatedItem = await _service.UpdateAsync(createdItem.Id, updateDto);

        // Assert
        updatedItem.Should().NotBeNull();
        updatedItem!.Title.Should().Be("Original Title"); // Title should remain unchanged
        updatedItem.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteItem()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "Test Item" };
        var createdItem = await _service.CreateAsync(createDto);

        // Act
        var deleteResult = await _service.DeleteAsync(createdItem.Id);

        // Assert
        deleteResult.Should().BeTrue();

        // Verify item is deleted
        var retrievedItem = await _service.GetByIdAsync(createdItem.Id);
        retrievedItem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var deleteResult = await _service.DeleteAsync(Guid.NewGuid());

        // Assert
        deleteResult.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_WithCompletedStatus_ShouldCreateCompletedItem()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = "Already Completed Task",
            IsCompleted = true
        };

        // Act
        var createdItem = await _service.CreateAsync(createDto);

        // Assert
        createdItem.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnItemsOrderedByCreatedDate()
    {
        // Arrange
        var firstItem = await _service.CreateAsync(new CreateTodoItemDto { Title = "First" });
        await Task.Delay(10); // Small delay to ensure different timestamps
        var secondItem = await _service.CreateAsync(new CreateTodoItemDto { Title = "Second" });

        // Act
        var items = (await _service.GetAllAsync()).ToList();

        // Assert
        var firstIndex = items.FindIndex(i => i.Id == firstItem.Id);
        var secondIndex = items.FindIndex(i => i.Id == secondItem.Id);
        firstIndex.Should().BeLessThan(secondIndex);
    }
}
