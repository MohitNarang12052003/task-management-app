using TaskManagement.Api.Models;

namespace TaskManagement.Api.DTOs;

public class TaskResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
