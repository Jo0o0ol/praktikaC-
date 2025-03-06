using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManagerApp
{
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum Status
    {
        Todo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DataManager
    {
        private static List<User> users = new List<User>();
        private static List<TaskItem> tasks = new List<TaskItem>();
        private static int nextTaskId = 1;
        private static int nextUserId = 1;

        public static async Task<bool> RegisterUserAsync(string username, string password)
        {
            if (users.Exists(u => u.Username == username))
            {
                return false;
            }

            User newUser = new User
            {
                Id = nextUserId++,
                Username = username,
                Password = password
            };

            users.Add(newUser);
            await Task.Delay(50);
            return true;
        }

        public static async Task<User> AuthenticateUserAsync(string username, string password)
        {
            await Task.Delay(50);
            return users.Find(u => u.Username == username && u.Password == password);
        }

        public static async Task<TaskItem> CreateTaskAsync(string title, string description, Priority priority, int userId)
        {
            TaskItem newTask = new TaskItem
            {
                Id = nextTaskId++,
                Title = title,
                Description = description,
                Priority = priority,
                Status = Status.Todo,
                UserId = userId
            };

            tasks.Add(newTask);
            await Task.Delay(50);
            return newTask;
        }

        public static async Task<List<TaskItem>> GetTasksForUserAsync(int userId)
        {
            await Task.Delay(50);
            return tasks.FindAll(t => t.UserId == userId);
        }

        public static async Task<TaskItem> GetTaskByIdAsync(int taskId)
        {
            await Task.Delay(50);
            return tasks.Find(t => t.Id == taskId);
        }

        public static async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            TaskItem existingTask = tasks.Find(t => t.Id == task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Priority = task.Priority;
                existingTask.Status = task.Status;
                await Task.Delay(50);
                return true;
            }
            return false;
        }

        public static async Task<bool> DeleteTaskAsync(int taskId)
        {
            TaskItem taskToRemove = tasks.Find(t => t.Id == taskId);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
                await Task.Delay(50);
                return true;
            }
            return false;
        }
    }

    class Program
    {
        private static User loggedInUser = null;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в Task Manager!");

            while (true)
            {
                if (loggedInUser == null)
                {
                    Console.WriteLine("\n1. Регистрация");
                    Console.WriteLine("2. Вход");
                    Console.WriteLine("3. Выход");

                    Console.Write("Выберите опцию: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await RegisterAsync();
                            break;
                        case "2":
                            await LoginAsync();
                            break;
                        case "3":
                            Console.WriteLine("Выход из Task Manager. До свидания!");
                            return;
                        default:
                            Console.WriteLine("Неверная опция. Пожалуйста, попробуйте еще раз.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"\nВы вошли как {loggedInUser.Username}");
                    Console.WriteLine("1. Создать задачу");
                    Console.WriteLine("2. Просмотреть задачи");
                    Console.WriteLine("3. Редактировать задачу");
                    Console.WriteLine("4. Удалить задачу");
                    Console.WriteLine("5. Выйти");

                    Console.Write("Выберите опцию: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await CreateTaskAsync();
                            break;
                        case "2":
                            await ViewTasksAsync();
                            break;
                        case "3":
                            await EditTaskAsync();
                            break;
                        case "4":
                            await DeleteTaskAsync();
                            break;
                        case "5":
                            Logout();
                            break;
                        default:
                            Console.WriteLine("Неверная опция. Пожалуйста, попробуйте еще раз.");
                            break;
                    }
                }
            }
        }

        static async Task RegisterAsync()
        {
            Console.Write("Введите имя пользователя: ");
            string username = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            bool success = await DataManager.RegisterUserAsync(username, password);

            if (success)
            {
                Console.WriteLine("Регистрация прошла успешно!");
            }
            else
            {
                Console.WriteLine("Регистрация не удалась. Имя пользователя может уже существовать.");
            }
        }

        static async Task LoginAsync()
        {
            Console.Write("Введите имя пользователя: ");
            string username = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            loggedInUser = await DataManager.AuthenticateUserAsync(username, password);

            if (loggedInUser != null)
            {
                Console.WriteLine("Вход выполнен успешно!");
            }
            else
            {
                Console.WriteLine("Вход не удался. Неверное имя пользователя или пароль.");
            }
        }

        static void Logout()
        {
            loggedInUser = null;
            Console.WriteLine("Вы вышли из системы.");
        }

        static async Task CreateTaskAsync()
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();
            Console.Write("Введите описание задачи: ");
            string description = Console.ReadLine();

            Console.WriteLine("Выберите приоритет (Low, Medium, High): ");
            string priorityString = Console.ReadLine();
            Priority priority;
            if (!Enum.TryParse(priorityString, true, out priority))
            {
                Console.WriteLine("Неверный приоритет. Установка на Medium.");
                priority = Priority.Medium;
            }

            TaskItem newTask = await DataManager.CreateTaskAsync(title, description, priority, loggedInUser.Id);
            Console.WriteLine($"Задача создана с ID: {newTask.Id}");
        }

        static async Task ViewTasksAsync()
        {
            List<TaskItem> tasks = await DataManager.GetTasksForUserAsync(loggedInUser.Id);

            if (tasks.Count == 0)
            {
                Console.WriteLine("Задачи не найдены.");
                return;
            }

            Console.WriteLine("Ваши задачи:");
            foreach (var task in tasks)
            {
                Console.WriteLine($"ID: {task.Id}, Название: {task.Title}, Статус: {task.Status}, Приоритет: {task.Priority}");
            }
        }

        static async Task EditTaskAsync()
        {
            Console.Write("Введите ID задачи для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Неверный ID задачи.");
                return;
            }

            TaskItem task = await DataManager.GetTaskByIdAsync(taskId);
            if (task == null || task.UserId != loggedInUser.Id)
            {
                Console.WriteLine("Задача не найдена или у вас нет прав на ее редактирование.");
                return;
            }

            Console.Write("Введите новое название задачи (оставьте пустым, чтобы оставить текущее): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTitle))
            {
                task.Title = newTitle;
            }

            Console.Write("Введите новое описание задачи (оставьте пустым, чтобы оставить текущее): ");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription))
            {
                task.Description = newDescription;
            }

            Console.WriteLine("Выберите новый приоритет (Low, Medium, High, оставьте пустым, чтобы оставить текущий): ");
            string newPriorityString = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPriorityString))
            {
                if (Enum.TryParse(newPriorityString, true, out Priority newPriority))
                {
                    task.Priority = newPriority;
                }
                else
                {
                    Console.WriteLine("Неверный приоритет. Сохранение текущего приоритета.");
                }
            }

            Console.WriteLine("Выберите новый статус (Todo, InProgress, Done, оставьте пустым, чтобы оставить текущий): ");
            string newStatusString = Console.ReadLine();
            if (!string.IsNullOrEmpty(newStatusString))
            {
                if (Enum.TryParse(newStatusString, true, out Status newStatus))
                {
                    task.Status = newStatus;
                }
                else
                {
                    Console.WriteLine("Неверный статус. Сохранение текущего статуса.");
                }
            }

            bool success = await DataManager.UpdateTaskAsync(task);
            if (success)
            {
                Console.WriteLine("Задача успешно обновлена!");
            }
            else
            {
                Console.WriteLine("Не удалось обновить задачу.");
            }
        }

        static async Task DeleteTaskAsync()
        {
            Console.Write("Введите ID задачи для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Неверный ID задачи.");
                return;
            }

            TaskItem task = await DataManager.GetTaskByIdAsync(taskId);
            if (task == null || task.UserId != loggedInUser.Id)
            {
                Console.WriteLine("Задача не найдена или у вас нет прав на ее удаление.");
                return;
            }

            bool success = await DataManager.DeleteTaskAsync(taskId);
            if (success)
            {
                Console.WriteLine("Задача успешно удалена!");
            }
            else
            {
                Console.WriteLine("Не удалось удалить задачу.");
            }
        }
    }
}