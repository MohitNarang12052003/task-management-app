using MongoDB.Driver;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskItem> _tasks;

    public TaskRepository(IMongoDatabase database, IConfiguration configuration)
    {
        var collectionName = configuration["MongoDbSettings:CollectionName"] ?? "Tasks";
        _tasks = database.GetCollection<TaskItem>(collectionName);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(string userId)
        => await _tasks.Find(t => t.UserId == userId).ToListAsync();

    public async Task<TaskItem?> GetByIdAsync(string id, string userId)
        => await _tasks.Find(t => t.Id == id && t.UserId == userId).FirstOrDefaultAsync();

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await _tasks.InsertOneAsync(task);
        return task;
    }

    public async Task<bool> UpdateAsync(string id, TaskItem task)
    {
        var result = await _tasks.ReplaceOneAsync(t => t.Id == id && t.UserId == task.UserId, task);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var result = await _tasks.DeleteOneAsync(t => t.Id == id && t.UserId == userId);
        return result.DeletedCount > 0;
    }
}
