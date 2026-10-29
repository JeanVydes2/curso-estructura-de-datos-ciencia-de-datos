namespace DataQualityIndex;

/// <summary>Proporciona operaciones estructurales y recorridos recursivos.</summary>
public sealed class BinaryTree<T>
{
    /// <summary>Raíz del árbol; es null cuando el árbol está vacío.</summary>
    public BinaryNode<T>? Root { get; private set; }

    /// <summary>Establece la raíz del árbol.</summary>
    public void SetRoot(BinaryNode<T>? root)
    {
        Root = root;
    }

    /// <summary>Cuenta recursivamente todos los nodos.</summary>
    public int CountNodes()
    {
        return CountNodes(Root);
    }

    /// <summary>Calcula la altura en cantidad de aristas.</summary>
    public int Height()
    {
        return Height(Root);
    }

    /// <summary>Cuenta recursivamente los nodos que no tienen hijos.</summary>
    public int CountLeaves()
    {
        return CountLeaves(Root);
    }

    /// <summary>Obtiene los nodos internos a partir del total y las hojas.</summary>
    public int CountInternal()
    {
        return CountNodes() - CountLeaves();
    }

    /// <summary>Recorre el árbol izquierda, raíz y derecha.</summary>
    public List<BinaryNode<T>> Inorder()
    {
        List<BinaryNode<T>> visited = new();
        CollectInorder(Root, visited);
        return visited;
    }

    /// <summary>Recorre el árbol raíz, izquierda y derecha.</summary>
    public List<BinaryNode<T>> Preorder()
    {
        List<BinaryNode<T>> visited = new();
        CollectPreorder(Root, visited);
        return visited;
    }

    /// <summary>Recorre el árbol izquierda, derecha y raíz.</summary>
    public List<BinaryNode<T>> Postorder()
    {
        List<BinaryNode<T>> visited = new();
        CollectPostorder(Root, visited);
        return visited;
    }

    /// <summary>Recorre el árbol por niveles desde la raíz.</summary>
    public List<BinaryNode<T>> LevelOrder()
    {
        List<BinaryNode<T>> visited = new();
        if (Root == null)
        {
            return visited;
        }

        Queue<BinaryNode<T>> pending = new();
        pending.Enqueue(Root);

        while (pending.Count > 0)
        {
            BinaryNode<T> node = pending.Dequeue();
            visited.Add(node);

            if (node.Left != null)
            {
                pending.Enqueue(node.Left);
            }

            if (node.Right != null)
            {
                pending.Enqueue(node.Right);
            }
        }

        return visited;
    }

    private static int CountNodes(BinaryNode<T>? node)
    {
        return node == null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    private static int Height(BinaryNode<T>? node)
    {
        return node == null ? -1 : 1 + Math.Max(Height(node.Left), Height(node.Right));
    }

    private static int CountLeaves(BinaryNode<T>? node)
    {
        if (node == null)
        {
            return 0;
        }

        if (node.Left == null && node.Right == null)
        {
            return 1;
        }
        return CountLeaves(node.Left) + CountLeaves(node.Right);
    }

    private static void CollectInorder(BinaryNode<T>? node, List<BinaryNode<T>> visited)
    {
        if (node == null)
        {
            return;
        }
        CollectInorder(node.Left, visited);
        visited.Add(node);
        CollectInorder(node.Right, visited);
    }

    private static void CollectPreorder(BinaryNode<T>? node, List<BinaryNode<T>> visited)
    {
        if (node == null)
        {
            return;
        }
        visited.Add(node);
        CollectPreorder(node.Left, visited);
        CollectPreorder(node.Right, visited);
    }

    private static void CollectPostorder(BinaryNode<T>? node, List<BinaryNode<T>> visited)
    {
        if (node == null)
        {
            return;
        }
        CollectPostorder(node.Left, visited);
        CollectPostorder(node.Right, visited);
        visited.Add(node);
    }
}
