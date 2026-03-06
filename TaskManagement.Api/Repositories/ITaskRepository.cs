using TaskManagement.Api.Models;

namespace TaskManagement.Api.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(string id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<bool> UpdateAsync(string id, TaskItem task);
    Task<bool> DeleteAsync(string id);
}
