using System;

public class Rectangle
{
    private double width;
    private double height;

    public Rectangle()
    {
        width = 0;
        height = 0;
        Console.WriteLine("Конструктор по умолчанию вызван");
    }

    public Rectangle(double side)
    {
        width = side;
        height = side;
        Console.WriteLine("Конструктор для квадрата вызван");
    }

    public Rectangle(double width, double height)
    {
        this.width = width;
        this.height = height;
        Console.WriteLine("Конструктор с шириной и высотой вызван");
    }

    ~Rectangle()
    {
        Console.WriteLine("Деструктор вызван");
    }

    public double CalculateArea()
    {
        return width * height;
    }

    public void DisplayArea()
    {
        Console.WriteLine($"Площадь прямоугольника: {CalculateArea()}");
    }
    public static void Main(string[] args)
    {
        Rectangle rect1 = new Rectangle();
        rect1.DisplayArea();

        Rectangle rect2 = new Rectangle(5);
        rect2.DisplayArea();

        Rectangle rect3 = new Rectangle(4, 6);
        rect3.DisplayArea();

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}