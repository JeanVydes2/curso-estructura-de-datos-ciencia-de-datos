namespace EtlScheduler;

/// <summary>
/// Dibuja el montículo de dos formas: como árbol y como la lista que realmente lo guarda.
/// En el árbol, los hijos de la posición i son 2i + 1 y 2i + 2. El elemento resaltado se
/// muestra entre &lt; &gt; y en amarillo, para que se vea aunque la consola no tenga colores.
/// </summary>
public static class HeapPrinter
{
    private const int Gap = 2;

    /// <summary>
    /// Dibuja el montículo como árbol y debajo la lista con sus índices.
    /// </summary>
    /// <param name="items">Lista del montículo, en el orden en que está guardada.</param>
    /// <param name="highlightIndex">Posición que se resalta con &lt; &gt; y en amarillo; -1 para no resaltar.</param>
    public static void Print<T>(List<T> items, int highlightIndex)
    {
        List<string> lines = RenderTree(items, highlightIndex);
        foreach (string line in lines)
        {
            WriteHighlighted(line);
        }

        Console.WriteLine();
        Console.Write("items: ");
        for (int index = 0; index < items.Count; index++)
        {
            string cell = "[" + index + "] " + items[index];
            if (index == highlightIndex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("<" + cell + ">");
                Console.ResetColor();
            }
            else
            {
                Console.Write(cell);
            }

            Console.Write("  ");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Genera el dibujo del árbol como texto. Los hijos de la posición i están en 2i + 1 y 2i + 2.
    /// </summary>
    /// <returns>Una línea de texto por cada fila del dibujo.</returns>
    public static List<string> RenderTree<T>(List<T> items, int highlightIndex)
    {
        List<string> result = new List<string>();
        if (items.Count == 0)
        {
            result.Add("(montículo vacío)");
            return result;
        }

        // Columna de cada posición según el orden inorden de los índices.
        List<int> inorder = new List<int>();
        CollectInorder(0, items.Count, inorder);

        int[] centers = new int[items.Count];
        int width = 0;
        foreach (int index in inorder)
        {
            string label = LabelOf(items, index, highlightIndex);
            centers[index] = width + label.Length / 2;
            width += label.Length + Gap;
        }

        int maxDepth = DepthOf(items.Count - 1);
        for (int depth = 0; depth <= maxDepth; depth++)
        {
            char[] nodeLine = NewLine(width);
            char[] edgeLine = NewLine(width);

            for (int index = 0; index < items.Count; index++)
            {
                if (DepthOf(index) != depth)
                {
                    continue;
                }

                string label = LabelOf(items, index, highlightIndex);
                int start = centers[index] - label.Length / 2;
                for (int offset = 0; offset < label.Length; offset++)
                {
                    nodeLine[start + offset] = label[offset];
                }

                DrawConnector(index, items.Count, centers, edgeLine);
            }

            result.Add(new string(nodeLine).TrimEnd());
            if (depth < maxDepth)
            {
                result.Add(new string(edgeLine).TrimEnd());
            }
        }

        return result;
    }

    /// <summary>
    /// Dibuja la unión entre la posición <paramref name="index"/> y sus hijos.
    /// </summary>
    private static void DrawConnector(int index, int count, int[] centers, char[] line)
    {
        int left = 2 * index + 1;
        int right = 2 * index + 2;
        int center = centers[index];

        if (left < count)
        {
            line[centers[left]] = '┌';
            for (int column = centers[left] + 1; column < center; column++)
            {
                line[column] = '─';
            }
        }

        if (right < count)
        {
            line[centers[right]] = '┐';
            for (int column = center + 1; column < centers[right]; column++)
            {
                line[column] = '─';
            }
        }

        if (left < count && right < count)
        {
            line[center] = '┴';
        }
        else if (left < count)
        {
            line[center] = '┘';
        }
    }

    /// <summary>
    /// Recorre las posiciones en inorden para asignar a cada una su columna en el dibujo.
    /// </summary>
    private static void CollectInorder(int index, int count, List<int> inorder)
    {
        if (index >= count)
        {
            return;
        }

        CollectInorder(2 * index + 1, count, inorder);
        inorder.Add(index);
        CollectInorder(2 * index + 2, count, inorder);
    }

    /// <summary>
    /// Calcula el nivel de una posición: ⌊log₂ (index + 1)⌋.
    /// </summary>
    private static int DepthOf(int index)
    {
        int depth = 0;
        int position = index + 1;
        while (position > 1)
        {
            position = position / 2;
            depth++;
        }

        return depth;
    }

    /// <summary>
    /// Construye la etiqueta de una posición: [X] normal o &lt;X&gt; resaltada.
    /// </summary>
    private static string LabelOf<T>(List<T> items, int index, int highlightIndex)
    {
        if (index == highlightIndex)
        {
            return "<" + items[index] + ">";
        }

        return "[" + items[index] + "]";
    }

    /// <summary>
    /// Crea una línea de dibujo llena de espacios.
    /// </summary>
    private static char[] NewLine(int width)
    {
        char[] line = new char[width];
        for (int column = 0; column < width; column++)
        {
            line[column] = ' ';
        }

        return line;
    }

    /// <summary>
    /// Escribe una línea y colorea en amarillo el tramo entre &lt; y &gt;.
    /// </summary>
    private static void WriteHighlighted(string line)
    {
        int open = line.IndexOf('<');
        int close = line.IndexOf('>');
        if (open < 0 || close < open)
        {
            Console.WriteLine(line);
            return;
        }

        Console.Write(line.Substring(0, open));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(line.Substring(open, close - open + 1));
        Console.ResetColor();
        Console.WriteLine(line.Substring(close + 1));
    }
}
