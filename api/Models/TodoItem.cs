namespace Api.Models;

/// <summary>
/// Represents a TODO item in the list
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Unique identifier for the TODO item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Title or description of the TODO task
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the task is completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Date and time when the TODO item was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the TODO item was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
