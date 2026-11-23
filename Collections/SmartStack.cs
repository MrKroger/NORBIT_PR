using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Умный стек.
/// </summary>
/// <typeparam name="T">Тип элементов в стеке.</typeparam>
public class SmartStack<T> : IEnumerable<T>, IEnumerable
{
    private const int DefaultCapacity = 4;
    private T[] _items;
    private int _count;

    /// <summary>
    /// Количество элементов в стеке.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Текущая емкость внутреннего массива.
    /// </summary>
    public int Capacity => _items.Length;

    #region Конструкторы
    /// <summary>
    /// Инициализирует новый экземпляр стека с начальной емкостью 4 элемента.
    /// </summary>
    public SmartStack(): this(DefaultCapacity)
    {
        // _items = new T[4];
    }

    /// <summary>
    /// Инициализирует новый экземпляр стека с заданной емкостью.
    /// </summary>
    /// <param name="capacity">Начальная емкость массива. Должна быть больше нуля.</param>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если емкость меньше или равна нулю.</exception>
    public SmartStack(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _items = new T[capacity];
    }

    /// <summary>
    /// Инициализирует новый экземпляр стека, копируя элементы из коллекции.
    /// Последний элемент коллекции становится вершиной стека.
    /// </summary>
    /// <param name="collection">Коллекция для копирования.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если коллекция равна null.</exception>
    public SmartStack(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        T[] tempArray = collection.ToArray();
        _items = new T[tempArray.Length > 0 ? tempArray.Length : DefaultCapacity];
        _count = tempArray.Length;

        // Заполняем стек в обратном порядке
        for (var i = 0; i < tempArray.Length; i++)
        {
            _items[i] = tempArray[tempArray.Length - 1 - i];
        }
    }
    #endregion 

    #region Основные операции
    /// <summary>
    /// Добавляет элемент на вершину стека. При нехватке места емкость удваивается.
    /// </summary>
    /// <param name="item">Добавляемый элемент.</param>
    public void Push(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count] = item;
        _count++;
    }

    /// <summary>
    /// Удаляет и возвращает элемент с вершины стека.
    /// </summary>
    /// <returns>Элемент с вершины стека.</returns>
    /// <exception cref="InvalidOperationException">Выбрасывается, если стек пуст.</exception>
    public T Pop()
    {
        if (_count == 0)
            throw new InvalidOperationException("Стек пуст. Невозможно извлечь элемент.");

        _count--;
        T item = _items[_count];
        
        _items[_count] = default!; 
        
        return item;
    }

    /// <summary>
    /// Возвращает элемент с вершины стека без его удаления.
    /// </summary>
    /// <returns>Элемент с вершины стека.</returns>
    /// <exception cref="InvalidOperationException">Выбрасывается, если стек пуст.</exception>
    public T Peek()
    {
        if (_count == 0)
            throw new InvalidOperationException("Стек пуст. Невозможно просмотреть элемент.");

        return _items[_count - 1];
    }

    #endregion

    #region Дополнительные методы
    /// <summary>
    /// Добавляет элементы коллекции на вершину стека. Последний элемент коллекции станет вершиной.
    /// </summary>
    /// <param name="collection">Коллекция добавляемых элементов.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если коллекция равна null.</exception>
    public void PushRange(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        T[] tempArray = collection.ToArray();
        if (tempArray.Length == 0) return;

        EnsureCapacity(_count + tempArray.Length);

        // Добавляем в обратном порядке, чтобы последний элемент стал вершиной
        for (var i = tempArray.Length - 1; i >= 0; i--)
        {
            _items[_count] = tempArray[i];
            _count++;
        }
    }

    /// <summary>
    /// Проверяет наличие элемента в стеке.
    /// </summary>
    /// <param name="item">Искомый элемент.</param>
    /// <returns>True, если элемент найден, иначе False.</returns>
    public bool Contains(T item)
    {
        for (var i = 0; i < _count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    /// <summary>
    /// Возвращает элемент по его глубине в стеке (0 - вершина, Count-1 - основание).
    /// </summary>
    /// <param name="index">Глубина элемента.</param>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если индекс выходит за пределы допустимых значений.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс выходит за границы стека.");

            // Так как вершина находится в конце массива, вычисляем реальный индекс
            return _items[_count - 1 - index];
        }
    }

    /// <summary>
    /// Увеличивает емкость внутреннего массива при необходимости (удваивает его).
    /// </summary>
    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity <= _items.Length) return;

        var newCapacity = _items.Length == 0 
            ? DefaultCapacity 
            : _items.Length * 2;
        
        // Если удвоенной емкости все равно не хватает (например, при PushRange)
        while (newCapacity < requiredCapacity)
        {
            newCapacity *= 2;
        }

        Array.Resize(ref _items, newCapacity);
    }

    /// <summary>
    /// Возвращает перечислитель, осуществляющий итерацию от вершины стека к основанию.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}