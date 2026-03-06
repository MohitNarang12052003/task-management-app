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

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _tasks.Find(_ => true).ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        return await _tasks.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await _tasks.InsertOneAsync(task);
        return task;
    }

    public async Task<bool> UpdateAsync(string id, TaskItem task)
    {
        var result = await _tasks.ReplaceOneAsync(t => t.Id == id, task);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _tasks.DeleteOneAsync(t => t.Id == id);
        return result.DeletedCount > 0;
    }
}
