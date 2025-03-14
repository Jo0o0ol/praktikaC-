using System;
using System.Collections.Generic;

public class User
{
    public string Name { get; private set; }

    public User(string name)
    {
        Name = name;
    }

    public virtual void DisplayOptions() { }
}

public class Librarian : User
{
    public Librarian(string name) : base(name) { }

    public override void DisplayOptions()
    {
        Console.WriteLine("1. Добавить книгу");
        Console.WriteLine("2. Удалить книгу");
        Console.WriteLine("3. Зарегистрировать пользователя");
        Console.WriteLine("4. Просмотреть пользователей");
        Console.WriteLine("5. Просмотреть книги");
    }

    public void AddBook(Library library, Book book)
    {
        library.AddBook(book);
    }

    public void RemoveBook(Library library, string title)
    {
        library.RemoveBook(title);
    }

    public void RegisterUser(Library library, User user)
    {
        library.RegisterUser(user); 
    }
}

public class RegularUser : User
{
    public RegularUser(string name) : base(name) { }

    public override void DisplayOptions()
    {
        Console.WriteLine("1. Просмотреть книги");
        Console.WriteLine("2. Взять книгу");
    }

    public void BorrowBook(Library library, string title)
    {
        library.BorrowBook(title, this);
    }
}

public class Book
{
    public string Title { get; private set; }
    public bool IsAvailable { get; private set; }

    public Book(string title)
    {
        Title = title;
        IsAvailable = true;
    }

    public void Borrow()
    {
        IsAvailable = false;
    }

    public void Return()
    {
        IsAvailable = true;
    }
}

public class Library
{
    private List<Book> books = new List<Book>();
    private List<User> users = new List<User>();

    public void AddBook(Book book)
    {
        books.Add(book);
        Console.WriteLine($"Книга '{book.Title}' добавлена.");
    }

    public void RemoveBook(string title)
    {
        Book book = books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null)
        {
            books.Remove(book);
            Console.WriteLine($"Книга '{title}' удалена.");
        }
        else
        {
            Console.WriteLine($"Книга '{title}' не найдена.");
        }
    }

    public void RegisterUser(User user)
    {
        users.Add(user);
        Console.WriteLine($"Пользователь '{user.Name}' зарегистрирован.");
    }

    public void DisplayUsers()
    {
        Console.WriteLine("Список пользователей:");
        foreach (var user in users)
        {
            Console.WriteLine(user.Name);
        }
    }

    public void DisplayBooks()
    {
        Console.WriteLine("Список книг:");
        foreach (var book in books)
        {
            string status = book.IsAvailable ? "Доступна" : "Выдана";
            Console.WriteLine($"'{book.Title}' - {status}");
        }
    }

    public void BorrowBook(string title, RegularUser user)
    {
        Book book = books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null && book.IsAvailable)
        {
            book.Borrow();
            Console.WriteLine($"Пользователь '{user.Name}' взял книгу '{title}'.");
        }
        else if (book != null)
        {
            Console.WriteLine($"Книга '{title}' уже выдана.");
        }
        else
        {
            Console.WriteLine($"Книга '{title}' не найдена.");
        }
    }
}

class Program
{
    static void Main()
    {
        Library library = new Library();
        Librarian librarian = new Librarian("Лариса");
        RegularUser user = new RegularUser("Александр");

        while (true)
        {
            Console.WriteLine("1 - Библиотекарь, 2 - Пользователь, 0 - Выйти");
            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 1)
            {
                librarian.DisplayOptions();
                int action = Convert.ToInt32(Console.ReadLine());

                switch (action)
                {
                    case 1:
                        Console.Write("Название книги: ");
                        string titleToAdd = Console.ReadLine();
                        librarian.AddBook(library, new Book(titleToAdd));
                        break;
                    case 2:
                        Console.Write("Название книги для удаления: ");
                        string titleToRemove = Console.ReadLine();
                        librarian.RemoveBook(library, titleToRemove);
                        break;
                    case 3:
                        Console.Write("Имя нового пользователя: ");
                        string userName = Console.ReadLine();
                        librarian.RegisterUser(library, new RegularUser(userName));
                        break;
                    case 4:
                        library.DisplayUsers();
                        break;
                    case 5:
                        library.DisplayBooks();
                        break;
                }
            }
            else if (choice == 2)
            {
                user.DisplayOptions();  
                int action = Convert.ToInt32(Console.ReadLine());

                switch (action)
                {
                    case 1:
                        library.DisplayBooks();
                        break;
                    case 2:
                        Console.Write("Название книги для взятия: ");
                        string titleToBorrow = Console.ReadLine();
                        user.BorrowBook(library, titleToBorrow);
                        break;
                }
            }
            else if (choice == 0)
            {
                break;
            }
        }
    }
}
