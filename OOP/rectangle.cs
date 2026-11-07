using System;

/// <summary>
/// Класс, представляющий геометрическую фигуру "Прямоугольник".
/// Обеспечивает нахождение объекта в заведомо корректном состоянии.
/// </summary>
public class Rectangle
{
    private double _x;
    private double _y;
    private double _width;
    private double _height;

    /// <summary>
    /// Координата X левого верхнего угла. Может быть отрицательной.
    /// </summary>
    public double X
    {
        get => _x;
        set => _x = value;
    }

    /// <summary>
    /// Координата Y левого верхнего угла. Может быть отрицательной.
    /// </summary>
    public double Y
    {
        get => _y;
        set => _y = value; 
    }

    /// <summary>
    /// Ширина прямоугольника. Должна быть строго больше нуля.
    /// </summary>
    public double Width
    {
        get => _width;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(Width));
            _width = value;
        }
    }

    /// <summary>
    /// Высота прямоугольника. Должна быть строго больше нуля.
    /// </summary>
    public double Height
    {
        get => _height;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(Height));
            _height = value;
        }
    }

    /// <summary>
    /// Вычисляет площадь прямоугольника.
    /// </summary>
    public double Area => Width * Height;

    /// <summary>
    /// Вычисляет периметр прямоугольника.
    /// </summary>
    public double Perimeter => 2 * (Width + Height);

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Rectangle"/>.
    /// </summary>
    public Rectangle(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}