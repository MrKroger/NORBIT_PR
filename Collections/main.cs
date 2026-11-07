class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Тестирование SmartStack ---");

        // 1. Конструктор без параметров (емкость 4)
        SmartStack<int> stack = new SmartStack<int>();
        Console.WriteLine($"Создан пустой стек. Емкость: {stack.Capacity}, Элементов: {stack.Count}");

        // 2. Push и удвоение емкости
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        Console.WriteLine($"\nПосле 4-х Push. Емкость: {stack.Capacity}, Элементов: {stack.Count}");
        stack.Push(50); // Должно вызвать удвоение
        Console.WriteLine($"После 5-го Push. Емкость: {stack.Capacity}, Элементов: {stack.Count}");

        // 3. Peek и индексер
        Console.WriteLine($"\nВершина (Peek): {stack.Peek()}");
        Console.WriteLine($"Элемент на глубине 0 (Индексатор): {stack[0]}"); // 50
        Console.WriteLine($"Элемент на глубине 1 (Индексатор): {stack[1]}"); // 40

        // 4. Contains
        Console.WriteLine($"Содержит 30? {stack.Contains(30)}");
        Console.WriteLine($"Содержит 99? {stack.Contains(99)}");

        // 5. Pop
        int popped = stack.Pop();
        Console.WriteLine($"\nИзвлечен (Pop): {popped}. Новый размер: {stack.Count}");
        Console.WriteLine($"Новая вершина: {stack.Peek()}");

        // 6. Обход (IEnumerable)
        Console.Write("\nОбход стека (от вершины к основанию): ");
        foreach (var item in stack)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();

        // 7. Конструктор с IEnumerable и PushRange
        Console.WriteLine("\n--- Тест конструктора с IEnumerable и PushRange ---");
        List<string> initialData = new List<string> { "A", "B", "C" };
        
        // Конструктор: "C" должно стать вершиной
        SmartStack<string> stringStack = new SmartStack<string>(initialData);
        Console.WriteLine($"Стек из коллекции (вершина должна быть C): {stringStack.Peek()}");

        // PushRange: "E", "F" должны лечь так, чтобы "F" стало вершиной
        List<string> moreData = new List<string> { "D", "E", "F" };
        stringStack.PushRange(moreData);
        
        Console.Write("Стек после PushRange (вершина F): ");
        foreach (var item in stringStack)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
        
        Console.ReadLine();
    }
}