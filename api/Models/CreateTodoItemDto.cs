using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Data transfer object for creating a new TODO item
/// </summary>
public class CreateTodoItemDto
{
    /// <summary>
    /// Title or description of the TODO task
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Indicates whether the task is completed (default is false)
    /// </summary>
    public bool IsCompleted { get; set; } = false;
}
