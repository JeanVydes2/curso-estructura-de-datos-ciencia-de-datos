namespace DataQualityIndex;

/// <summary>Árbol binario que mantiene claves únicas y ordenadas.</summary>
public sealed class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
{
    /// <summary>Obtiene la clave que se compara para cada valor.</summary>
    private readonly Func<TValue, TKey> keySelector;

    /// <summary>Operaciones estructurales y recorridos del árbol.</summary>
    public BinaryTree<TValue> Tree { get; } = new();

    /// <summary>Obtiene la cantidad de valores almacenados.</summary>
    public int Count => Tree.CountNodes();

    /// <summary>Crea un árbol usando una función para extraer la clave.</summary>
    public BinarySearchTree(Func<TValue, TKey> keySelector)
    {
        this.keySelector = keySelector;
    }

    /// <summary>Inserta un valor; devuelve false si la clave ya existe.</summary>
    public bool Insert(TValue value)
    {
        if (Tree.Root == null)
        {
            Tree.SetRoot(new BinaryNode<TValue>(value));
            return true;
        }

        BinaryNode<TValue> current = Tree.Root;
        while (true)
        {
            // 0 = equal, < 0 = left, > 0 = right
            // menor a 0 significa que la clave del valor a insertar es menor que la clave del nodo actual
            // mayor a 0 significa que la clave del valor a insertar es mayor que la clave del nodo actual
            int comparison = keySelector(value).CompareTo(keySelector(current.Value));
            if (comparison == 0) return false;

            if (comparison < 0)
            {
                if (current.Left == null)
                {
                    current.Left = new BinaryNode<TValue>(value);
                    return true;
                }
                current = current.Left;
            }
            else
            {
                if (current.Right == null)
                {
                    current.Right = new BinaryNode<TValue>(value);
                    return true;
                }
                current = current.Right;
            }
        }
    }

    /// <summary>Busca una clave siguiendo un único camino desde la raíz.</summary>
    public BinaryNode<TValue>? Find(TKey key)
    {
        BinaryNode<TValue>? current = Tree.Root;
        while (current != null)
        {
            int comparison = key.CompareTo(keySelector(current.Value));
            if (comparison == 0) return current;
            current = comparison < 0 ? current.Left : current.Right;
        }
        return null;
    }

    /// <summary>Devuelve los nodos comparados durante una búsqueda.</summary>
    public List<BinaryNode<TValue>> SearchPath(TKey key)
    {
        List<BinaryNode<TValue>> path = new();
        BinaryNode<TValue>? current = Tree.Root;
        while (current != null)
        {
            path.Add(current);
            int comparison = key.CompareTo(keySelector(current.Value));
            if (comparison == 0) break;
            current = comparison < 0 ? current.Left : current.Right;
        }
        return path;
    }

    /// <summary>Obtiene el nodo con la clave mínima.</summary>
    public BinaryNode<TValue>? Min()
    {
        BinaryNode<TValue>? current = Tree.Root;
        if (current == null) return null;
        while (current.Left != null) current = current.Left;
        return current;
    }

    /// <summary>Obtiene el nodo con la clave máxima.</summary>
    public BinaryNode<TValue>? Max()
    {
        BinaryNode<TValue>? current = Tree.Root;
        if (current == null) return null;
        while (current.Right != null) current = current.Right;
        return current;
    }

    /// <summary>Devuelve los valores de un intervalo en orden creciente.</summary>
    public List<TValue> Range(TKey low, TKey high)
    {
        List<TValue> result = new();
        CollectRange(Tree.Root, low, high, result);
        return result;
    }

    private void CollectRange(BinaryNode<TValue>? node, TKey low, TKey high, List<TValue> result)
    {
        if (node == null) return;
        TKey key = keySelector(node.Value);
        if (low.CompareTo(key) < 0) CollectRange(node.Left, low, high, result);
        if (low.CompareTo(key) <= 0 && high.CompareTo(key) >= 0) result.Add(node.Value);
        if (high.CompareTo(key) > 0) CollectRange(node.Right, low, high, result);
    }

    /// <summary>Comprueba recursivamente la propiedad global de orden.</summary>
    public bool IsValid() { return IsValid(Tree.Root, null, null); }

    private bool IsValid(BinaryNode<TValue>? node, BinaryNode<TValue>? lower, BinaryNode<TValue>? upper)
    {
        if (node == null) return true;
        TKey key = keySelector(node.Value);
        if (lower != null && key.CompareTo(keySelector(lower.Value)) <= 0) return false;
        if (upper != null && key.CompareTo(keySelector(upper.Value)) >= 0) return false;
        return IsValid(node.Left, lower, node) && IsValid(node.Right, node, upper);
    }
}
