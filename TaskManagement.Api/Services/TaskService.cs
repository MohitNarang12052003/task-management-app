using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Api.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(string userId)
    {
        var tasks = await _repository.GetAllAsync(userId);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(string id, string userId)
    {
        var task = await _repository.GetByIdAsync(id, userId);
        return task is null ? null : MapToDto(task);
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string userId)
    {
        var task = new TaskItem
        {
            Title       = dto.Title,
            Description = dto.Description,
            Status      = dto.Status,
            UserId      = userId,
            CreatedAt   = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(task);
        return MapToDto(created);
    }

    public async Task<bool> UpdateTaskAsync(string id, UpdateTaskDto dto, string userId)
    {
        var existing = await _repository.GetByIdAsync(id, userId);
        if (existing is null) return false;

        existing.Title       = dto.Title;
        existing.Description = dto.Description;
        existing.Status      = dto.Status;

        return await _repository.UpdateAsync(id, existing);
    }

    public async Task<bool> DeleteTaskAsync(string id, string userId)
        => await _repository.DeleteAsync(id, userId);

    private static TaskResponseDto MapToDto(TaskItem task) => new()
    {
        Id          = task.Id ?? string.Empty,
        Title       = task.Title,
        Description = task.Description,
        Status      = task.Status,
        CreatedAt   = task.CreatedAt
    };
}
