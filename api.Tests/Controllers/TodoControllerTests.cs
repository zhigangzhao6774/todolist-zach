using Api.Controllers;
using Api.Models;
using Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Api.Tests.Controllers;

public class TodoControllerTests
{
    private readonly Mock<ITodoService> _mockService;
    private readonly Mock<ILogger<TodoController>> _mockLogger;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mockService = new Mock<ITodoService>();
        _mockLogger = new Mock<ILogger<TodoController>>();
        _controller = new TodoController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAllItems()
    {
        // Arrange
        var expectedItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Item 1" },
            new TodoItem { Id = Guid.NewGuid(), Title = "Item 2" }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var items = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>().Subject;
        items.Should().HaveCount(2);
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkWithItem()
    {
        // Arrange
        var expectedItem = new TodoItem { Id = Guid.NewGuid(), Title = "Test Item" };
        _mockService.Setup(s => s.GetByIdAsync(expectedItem.Id)).ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.GetById(expectedItem.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var item = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        item.Id.Should().Be(expectedItem.Id);
        _mockService.Verify(s => s.GetByIdAsync(expectedItem.Id), Times.Once);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        _mockService.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "New Item", IsCompleted = false };
        var createdItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            IsCompleted = createDto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdItem);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        var item = createdResult.Value.Should().BeOfType<TodoItem>().Subject;
        item.Id.Should().Be(createdItem.Id);
        item.Title.Should().Be(createDto.Title);
        _mockService.Verify(s => s.CreateAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidModel_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateTodoItemDto { Title = "" }; // Invalid: empty title
        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _mockService.Verify(s => s.CreateAsync(It.IsAny<CreateTodoItemDto>()), Times.Never);
    }

    [Fact]
    public async Task Update_WithValidIdAndDto_ShouldReturnOkWithUpdatedItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateTodoItemDto { Title = "Updated Title", IsCompleted = true };
        var updatedItem = new TodoItem
        {
            Id = id,
            Title = updateDto.Title!,
            IsCompleted = updateDto.IsCompleted!.Value,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync(updatedItem);

        // Act
        var result = await _controller.Update(id, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var item = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        item.Id.Should().Be(id);
        item.Title.Should().Be(updateDto.Title);
        _mockService.Verify(s => s.UpdateAsync(id, updateDto), Times.Once);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateTodoItemDto { Title = "Updated Title" };
        _mockService.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _controller.Update(id, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        _mockService.Verify(s => s.UpdateAsync(id, updateDto), Times.Once);
    }

    [Fact]
    public async Task Update_WithInvalidModel_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateTodoItemDto { Title = "" }; // Invalid: empty title
        _controller.ModelState.AddModelError("Title", "Title must be between 1 and 200 characters");

        // Act
        var result = await _controller.Update(id, updateDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTodoItemDto>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }
}
