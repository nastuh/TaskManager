using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public async Task<int> AddTaskAsync(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название задачи не может быть пустым", nameof(title));

            var task = new TaskItem
            {
                Title = title.Trim(),
                Description = description?.Trim() ?? string.Empty
            };

            return await _taskRepository.CreateAsync(task);
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<bool> UpdateTaskCompletionAsync(int taskId, bool isCompleted)
        {
            var existingTask = await _taskRepository.GetByIdAsync(taskId);
            if (existingTask == null)
                throw new ArgumentException($"Задача с ID {taskId} не найдена");

            return await _taskRepository.UpdateCompletionStatusAsync(taskId, isCompleted);
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(taskId);
            if (existingTask == null)
                throw new ArgumentException($"Задача с ID {taskId} не найдена");

            return await _taskRepository.DeleteAsync(taskId);
        }

        public async Task<TaskItem?> GetTaskAsync(int taskId)
        {
            return await _taskRepository.GetByIdAsync(taskId);
        }
    }
}