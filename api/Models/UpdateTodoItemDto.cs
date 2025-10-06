using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Data transfer object for updating an existing TODO item
/// </summary>
public class UpdateTodoItemDto
{
    /// <summary>
    /// Title or description of the TODO task
    /// </summary>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string? Title { get; set; }

    /// <summary>
    /// Indicates whether the task is completed
    /// </summary>
    public bool? IsCompleted { get; set; }
}
