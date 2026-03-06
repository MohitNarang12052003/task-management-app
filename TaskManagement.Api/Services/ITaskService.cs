using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<TaskResponseDto?> GetTaskByIdAsync(string id);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto);
    Task<bool> UpdateTaskAsync(string id, UpdateTaskDto dto);
    Task<bool> DeleteTaskAsync(string id);
}
