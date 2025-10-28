using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(int id);
        Task<List<TaskItem>> GetAllAsync();
        Task<int> CreateAsync(TaskItem task);
        Task<bool> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateCompletionStatusAsync(int id, bool isCompleted);
    }
}