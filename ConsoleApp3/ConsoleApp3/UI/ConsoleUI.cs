using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Utils;

namespace TaskManager.UI
{
    public class ConsoleUI
    {
        private readonly TaskService _taskService;

        public ConsoleUI(TaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public async Task RunAsync()
        {
            Console.WriteLine("=== Управление задачами ===");

            while (true)
            {
                DisplayMenu();
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await AddTaskAsync();
                            break;
                        case "2":
                            await ViewAllTasksAsync();
                            break;
                        case "3":
                            await UpdateTaskStatusAsync();
                            break;
                        case "4":
                            await DeleteTaskAsync();
                            break;
                        case "5":
                            Console.WriteLine("Выход из приложения...");
                            return;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Просмотреть все задачи");
            Console.WriteLine("3. Обновить статус задачи");
            Console.WriteLine("4. Удалить задачу");
            Console.WriteLine("5. Выход");
            Console.Write("Ваш выбор: ");
        }

        private async Task AddTaskAsync()
        {
            Console.WriteLine("\n--- Добавление новой задачи ---");

            string title;
            do
            {
                Console.Write("Введите название задачи: ");
                title = Console.ReadLine() ?? string.Empty;

                if (!InputValidator.ValidateTitle(title))
                {
                    Console.WriteLine("Ошибка: название не может быть пустым и должно быть не длиннее 100 символов.");
                }
            } while (!InputValidator.ValidateTitle(title));

            Console.Write("Введите описание задачи (необязательно): ");
            var description = InputValidator.ValidateDescription(Console.ReadLine());

            try
            {
                var taskId = await _taskService.AddTaskAsync(title, description ?? string.Empty);
                Console.WriteLine($"Задача успешно добавлена с ID: {taskId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении задачи: {ex.Message}");
            }
        }

        private async Task ViewAllTasksAsync()
        {
            Console.WriteLine("\n--- Список всех задач ---");

            try
            {
                var tasks = await _taskService.GetAllTasksAsync();

                if (!tasks.Any())
                {
                    Console.WriteLine("Задачи не найдены.");
                    return;
                }

                foreach (var task in tasks)
                {
                    DisplayTask(task);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении задач: {ex.Message}");
            }
        }

        private async Task UpdateTaskStatusAsync()
        {
            Console.WriteLine("\n--- Обновление статуса задачи ---");

            int taskId;
            do
            {
                Console.Write("Введите ID задачи: ");
                var input = Console.ReadLine();

                if (!InputValidator.TryParseInt(input, out taskId))
                {
                    Console.WriteLine("Ошибка: введите корректный положительный числовой ID.");
                }
            } while (taskId <= 0);

            bool isCompleted;
            bool isValidInput;
            do
            {
                Console.Write("Задача выполнена? (да/нет): ");
                var input = Console.ReadLine();

                isValidInput = InputValidator.TryParseBool(input, out isCompleted);
                if (!isValidInput)
                {
                    Console.WriteLine("Ошибка: введите 'да' или 'нет'.");
                }
            } while (!isValidInput);

            try
            {
                var success = await _taskService.UpdateTaskCompletionAsync(taskId, isCompleted);
                if (success)
                {
                    Console.WriteLine($"Статус задачи с ID {taskId} успешно обновлен.");
                }
                else
                {
                    Console.WriteLine($"Не удалось обновить статус задачи с ID {taskId}.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }

        private async Task DeleteTaskAsync()
        {
            Console.WriteLine("\n--- Удаление задачи ---");

            int taskId;
            do
            {
                Console.Write("Введите ID задачи для удаления: ");
                var input = Console.ReadLine();

                if (!InputValidator.TryParseInt(input, out taskId))
                {
                    Console.WriteLine("Ошибка: введите корректный положительный числовой ID.");
                }
            } while (taskId <= 0);

            try
            {
                var task = await _taskService.GetTaskAsync(taskId);
                if (task != null)
                {
                    DisplayTask(task);
                    Console.Write("Вы уверены, что хотите удалить эту задачу? (да/нет): ");
                    var confirmation = Console.ReadLine()?.ToLower();

                    if (confirmation == "да" || confirmation == "yes" || confirmation == "y")
                    {
                        var success = await _taskService.DeleteTaskAsync(taskId);
                        if (success)
                        {
                            Console.WriteLine($"Задача с ID {taskId} успешно удалена.");
                        }
                        else
                        {
                            Console.WriteLine($"Не удалось удалить задачу с ID {taskId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Удаление отменено.");
                    }
                }
                else
                {
                    Console.WriteLine($"Задача с ID {taskId} не найдена.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
            }
        }

        private void DisplayTask(TaskItem task)
        {
            var status = task.IsCompleted ? "[✓]" : "[ ]";
            Console.WriteLine($"{status} ID: {task.Id}");
            Console.WriteLine($"    Название: {task.Title}");
            if (!string.IsNullOrEmpty(task.Description))
            {
                Console.WriteLine($"    Описание: {task.Description}");
            }
            Console.WriteLine($"    Создана: {task.CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine();
        }
    }
}