using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
    public class FileTaskRepository : ITaskRepository
    {
        private readonly string _filePath;
        private List<TaskItem> _tasks;
        private int _nextId = 1;

        public FileTaskRepository(string filePath)
        {
            _filePath = filePath;
            _tasks = new List<TaskItem>();
            LoadTasksAsync().Wait();
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var lines = await File.ReadAllLinesAsync(_filePath);
                    _tasks = new List<TaskItem>();

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var parts = line.Split('|');
                        if (parts.Length >= 5)
                        {
                            var task = new TaskItem
                            {
                                Id = int.Parse(parts[0]),
                                Title = parts[1],
                                Description = parts[2],
                                IsCompleted = bool.Parse(parts[3]),
                                CreatedAt = DateTime.Parse(parts[4])
                            };
                            _tasks.Add(task);

                            if (task.Id >= _nextId)
                                _nextId = task.Id + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки задач: {ex.Message}");
                _tasks = new List<TaskItem>();
            }
        }

        private async Task SaveTasksAsync()
        {
            try
            {
                var lines = _tasks.Select(task =>
                    $"{task.Id}|{task.Title}|{task.Description}|{task.IsCompleted}|{task.CreatedAt:O}"
                );
                await File.WriteAllLinesAsync(_filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения задач: {ex.Message}");
                throw;
            }
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _tasks.OrderByDescending(t => t.CreatedAt).ToList();
        }

        public async Task<int> CreateAsync(TaskItem task)
        {
            task.Id = _nextId++;
            task.CreatedAt = DateTime.Now;
            _tasks.Add(task);
            await SaveTasksAsync();
            return task.Id;
        }

        public async Task<bool> UpdateAsync(TaskItem task)
        {
            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.IsCompleted = task.IsCompleted;
                await SaveTasksAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                await SaveTasksAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateCompletionStatusAsync(int id, bool isCompleted)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsCompleted = isCompleted;
                await SaveTasksAsync();
                return true;
            }
            return false;
        }
    }
}