using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(string userId);
    Task<TaskResponseDto?> GetTaskByIdAsync(string id, string userId);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string userId);
    Task<bool> UpdateTaskAsync(string id, UpdateTaskDto dto, string userId);
    Task<bool> DeleteTaskAsync(string id, string userId);
}
