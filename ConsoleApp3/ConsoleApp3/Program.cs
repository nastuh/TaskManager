using TaskManager.Repositories;
using TaskManager.Services;
using TaskManager.UI;

namespace TaskManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var filePath = "tasks.txt";

            try
            {
                Console.WriteLine("=== Управление задачами ===");
                Console.WriteLine($"Данные сохраняются в файл: {Path.GetFullPath(filePath)}");

                // Dependency Injection (вручную)
                var taskRepository = new FileTaskRepository(filePath);
                var taskService = new TaskService(taskRepository);
                var consoleUi = new ConsoleUI(taskService);

                // Запуск приложения
                await consoleUi.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}