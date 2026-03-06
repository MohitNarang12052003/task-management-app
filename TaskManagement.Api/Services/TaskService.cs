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

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
    {
        var tasks = await _repository.GetAllAsync();
        return tasks.Select(MapToDto);
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(string id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task is null ? null : MapToDto(task);
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(task);
        return MapToDto(created);
    }

    public async Task<bool> UpdateTaskAsync(string id, UpdateTaskDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Status = dto.Status;

        return await _repository.UpdateAsync(id, existing);
    }

    public async Task<bool> DeleteTaskAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static TaskResponseDto MapToDto(TaskItem task) => new()
    {
        Id = task.Id ?? string.Empty,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        CreatedAt = task.CreatedAt
    };
}
