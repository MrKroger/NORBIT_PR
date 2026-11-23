using System;

/// <summary>
/// Прямоугольник.
/// </summary>
public class Rectangle
{
    private double _x;
    private double _y;
    private double _width;
    private double _height;

    /// <summary>
    /// Координата X левого верхнего угла.
    /// </summary>
    public double X
    {
        get => _x;
        set => _x = value;
    }

    /// <summary>
    /// Координата Y левого верхнего угла.
    /// </summary>
    public double Y
    {
        get => _y;
        set => _y = value; 
    }

    /// <summary>
    /// Ширина прямоугольника.
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
    /// Высота прямоугольника.
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
    public override string ToString()
    {
        return
            $"Прямоугольник с координатами ({X}, {Y})"+
            $"Размеры: {Width} x {Height}"+
            $"Площадь: {Area:f2}"+
            $"Периметр: {Perimeter:f2}";
    }
}