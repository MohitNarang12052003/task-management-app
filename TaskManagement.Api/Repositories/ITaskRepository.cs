using TaskManagement.Api.Models;

namespace TaskManagement.Api.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(string userId);
    Task<TaskItem?> GetByIdAsync(string id, string userId);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<bool> UpdateAsync(string id, TaskItem task);
    Task<bool> DeleteAsync(string id, string userId);
}
