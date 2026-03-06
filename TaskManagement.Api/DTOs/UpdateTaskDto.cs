using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.DTOs;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TaskItemStatus Status { get; set; }
}
