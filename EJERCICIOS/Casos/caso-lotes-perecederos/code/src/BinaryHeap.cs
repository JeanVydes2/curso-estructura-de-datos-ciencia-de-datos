namespace EtlScheduler;

/// <summary>
/// Montículo binario sobre List&lt;T&gt;. La raíz es el menor según el comparador.
/// Índices: hijos de i en 2i + 1 y 2i + 2; padre de i en (i - 1) / 2.
/// Invariante: ningún elemento es menor que su padre según el comparador.
/// </summary>
public class BinaryHeap<T>
{
    private readonly List<T> items = new List<T>();
    private readonly IComparer<T> comparer;

    /// <summary>
    /// Crea un montículo vacío.
    /// </summary>
    /// <param name="comparer">Criterio que decide qué elemento va primero.</param>
    public BinaryHeap(IComparer<T> comparer)
    {
        this.comparer = comparer;
    }

    /// <summary>
    /// Cantidad de elementos guardados.
    /// </summary>
    public int Count
    {
        get
        {
            return items.Count;
        }
    }

    /// <summary>
    /// Indica si el montículo no tiene elementos.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            return items.Count == 0;
        }
    }

    /// <summary>
    /// Altura del árbol que forma la lista: ⌊log₂ Count⌋, o -1 si está vacío.
    /// </summary>
    /// <remarks>
    /// Es la cantidad máxima de intercambios de Insert y ExtractRoot.
    /// </remarks>
    public int Height
    {
        get
        {
            int height = -1;
            int remaining = items.Count;
            while (remaining > 0)
            {
                remaining = remaining / 2;
                height++;
            }

            return height;
        }
    }

    /// <summary>
    /// Intercambios que hizo la última operación Insert o ExtractRoot.
    /// </summary>
    public int LastSwaps { get; private set; }

    /// <summary>
    /// Consulta el elemento de la raíz sin sacarlo.
    /// </summary>
    /// <remarks>
    /// Lanza <c>InvalidOperationException</c> si el montículo está vacío.
    /// </remarks>
    /// <returns>El elemento que va primero según el comparador.</returns>
    public T Peek()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("El montículo está vacío.");
        }

        return items[0];
    }

    /// <summary>
    /// Agrega un elemento al final de la lista y lo sube mientras vaya antes que su padre.
    /// </summary>
    /// <param name="item">Elemento que se agrega.</param>
    public void Insert(T item)
    {
        LastSwaps = 0;
        items.Add(item);
        SiftUp(items.Count - 1);
    }

    /// <summary>
    /// Saca la raíz, lleva el último elemento a la raíz y lo baja por el hijo que va primero.
    /// </summary>
    /// <remarks>
    /// Lanza <c>InvalidOperationException</c> si el montículo está vacío.
    /// </remarks>
    /// <returns>El elemento que estaba en la raíz.</returns>
    public T ExtractRoot()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("El montículo está vacío.");
        }

        LastSwaps = 0;
        T root = items[0];
        int lastIndex = items.Count - 1;
        items[0] = items[lastIndex];
        items.RemoveAt(lastIndex);

        if (items.Count > 0)
        {
            SiftDown(0);
        }

        return root;
    }

    /// <summary>
    /// Devuelve una copia de la lista en el orden en que está guardada.
    /// </summary>
    public List<T> Snapshot()
    {
        return new List<T>(items);
    }

    /// <summary>
    /// Comprueba que ningún elemento vaya antes que su padre, que está en la posición (i - 1) / 2.
    /// </summary>
    public bool IsValid()
    {
        for (int index = 1; index < items.Count; index++)
        {
            int parent = (index - 1) / 2;
            if (comparer.Compare(items[index], items[parent]) < 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Sube el elemento de <paramref name="index"/> mientras vaya estrictamente antes que su padre.
    /// </summary>
    private void SiftUp(int index)
    {
        int current = index;
        while (current > 0)
        {
            int parent = (current - 1) / 2;
            if (comparer.Compare(items[current], items[parent]) >= 0)
            {
                break;
            }

            Swap(current, parent);
            current = parent;
        }
    }

    /// <summary>
    /// Baja el elemento de <paramref name="index"/> intercambiándolo con el hijo que va primero, mientras ese hijo vaya antes que él.
    /// </summary>
    private void SiftDown(int index)
    {
        int current = index;
        while (true)
        {
            int left = 2 * current + 1;
            int right = 2 * current + 2;
            int smallest = current;

            if (left < items.Count && comparer.Compare(items[left], items[smallest]) < 0)
            {
                smallest = left;
            }

            if (right < items.Count && comparer.Compare(items[right], items[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest == current)
            {
                break;
            }

            Swap(current, smallest);
            current = smallest;
        }
    }

    /// <summary>
    /// Intercambia dos posiciones de la lista y cuenta el intercambio.
    /// </summary>
    private void Swap(int first, int second)
    {
        T temporary = items[first];
        items[first] = items[second];
        items[second] = temporary;
        LastSwaps++;
    }
}
