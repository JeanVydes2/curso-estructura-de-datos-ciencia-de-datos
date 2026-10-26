namespace ClasificacionBecas;

/// <summary>
/// Dibuja un árbol binario de arriba hacia abajo en la consola.
/// Cada nodo ocupa una columna según su posición en inorden, así un hijo izquierdo
/// siempre queda a la izquierda de su padre y uno derecho, a la derecha.
/// La etiqueta de cada nodo es Valor.ToString(): [X] normal, (X) marcado y &lt;X&gt; actual.
/// Las marcas se ven aunque la consola no muestre colores.
/// </summary>
/// <typeparam name="T">Tipo del dato que guardan los nodos.</typeparam>
public static class ImpresorConsola<T>
{
    private const int Separacion = 2;
    private const int Normal = 0;
    private const int Marcado = 1;
    private const int Actual = 2;

    /// <summary>
    /// Dibuja el árbol en la consola resaltando un camino y el nodo actual.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    /// <param name="marcados">Nodos que se muestran como recorridos: (X) y en cian.</param>
    /// <param name="actual">Nodo que se muestra como actual: &lt;X&gt; y en amarillo. Puede ser <c>null</c>.</param>
    public static void Imprimir(NodoBinario<T>? raiz, HashSet<NodoBinario<T>> marcados, NodoBinario<T>? actual)
    {
        List<char[]> lineas = new List<char[]>();
        List<int[]> colores = new List<int[]>();
        Construir(raiz, marcados, actual, lineas, colores);

        for (int fila = 0; fila < lineas.Count; fila++)
        {
            EscribirConColor(lineas[fila], colores[fila]);
        }
    }

    /// <summary>
    /// Dibuja el árbol en la consola sin resaltar ningún nodo.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    public static void Imprimir(NodoBinario<T>? raiz)
    {
        Imprimir(raiz, new HashSet<NodoBinario<T>>(), null);
    }

    /// <summary>
    /// Dibuja el árbol resaltando un conjunto de nodos recorridos.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    /// <param name="marcados">Nodos que se muestran como recorridos: (X) y en cian.</param>
    public static void Imprimir(NodoBinario<T>? raiz, HashSet<NodoBinario<T>> marcados)
    {
        Imprimir(raiz, marcados, null);
    }

    /// <summary>
    /// Dibuja el árbol resaltando un nodo actual.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    /// <param name="actual">Nodo que se muestra como actual: &lt;X&gt; y en amarillo.</param>
    public static void ImprimirActual(NodoBinario<T>? raiz, NodoBinario<T>? actual)
    {
        Imprimir(raiz, new HashSet<NodoBinario<T>>(), actual);
    }

    /// <summary>
    /// Genera el dibujo del árbol como texto, sin colores ni marcas.
    /// </summary>
    /// <param name="raiz">Raíz del árbol.</param>
    /// <returns>Una línea de texto por cada fila del dibujo. Se usa en las pruebas.</returns>
    public static List<string> RenderizarSimple(NodoBinario<T>? raiz)
    {
        List<char[]> lineas = new List<char[]>();
        List<int[]> colores = new List<int[]>();
        Construir(raiz, new HashSet<NodoBinario<T>>(), null, lineas, colores);

        List<string> resultado = new List<string>();
        foreach (char[] linea in lineas)
        {
            resultado.Add(new string(linea).TrimEnd());
        }

        return resultado;
    }

    /// <summary>
    /// Calcula el dibujo completo: la columna de cada nodo, una línea de nodos por nivel y, debajo, una línea de conectores.
    /// </summary>
    /// <remarks>
    /// La columna de cada nodo sale de su posición en inorden; por eso un hijo izquierdo siempre queda a la izquierda de su padre.
    /// </remarks>
    private static void Construir(NodoBinario<T>? raiz, HashSet<NodoBinario<T>> marcados, NodoBinario<T>? actual,
        List<char[]> lineas, List<int[]> colores)
    {
        if (raiz == null)
        {
            lineas.Add("(árbol vacío)".ToCharArray());
            colores.Add(new int[lineas[0].Length]);
            return;
        }

        Dictionary<NodoBinario<T>, int> centros = new Dictionary<NodoBinario<T>, int>();
        Dictionary<NodoBinario<T>, int> profundidades = new Dictionary<NodoBinario<T>, int>();
        List<NodoBinario<T>> inorden = new List<NodoBinario<T>>();
        RecorrerInorden(raiz, 0, inorden, profundidades);

        int ancho = 0;
        int profundidadMaxima = 0;
        foreach (NodoBinario<T> nodo in inorden)
        {
            string etiqueta = EtiquetaDe(nodo, Normal);
            centros[nodo] = ancho + etiqueta.Length / 2;
            ancho += etiqueta.Length + Separacion;
            if (profundidades[nodo] > profundidadMaxima)
            {
                profundidadMaxima = profundidades[nodo];
            }
        }

        // Por cada nivel hay una línea de nodos y, debajo, una línea de conectores.
        for (int profundidad = 0; profundidad <= profundidadMaxima; profundidad++)
        {
            char[] lineaNodos = NuevaLinea(ancho);
            int[] coloresNodos = new int[ancho];
            char[] lineaAristas = NuevaLinea(ancho);
            int[] coloresAristas = new int[ancho];

            foreach (NodoBinario<T> nodo in inorden)
            {
                if (profundidades[nodo] != profundidad)
                {
                    continue;
                }

                int color = ColorDe(nodo, marcados, actual);
                string etiqueta = EtiquetaDe(nodo, color);
                int inicio = centros[nodo] - etiqueta.Length / 2;
                for (int desplazamiento = 0; desplazamiento < etiqueta.Length; desplazamiento++)
                {
                    lineaNodos[inicio + desplazamiento] = etiqueta[desplazamiento];
                    coloresNodos[inicio + desplazamiento] = color;
                }

                DibujarConector(nodo, centros, marcados, lineaAristas, coloresAristas);
            }

            lineas.Add(lineaNodos);
            colores.Add(coloresNodos);
            if (profundidad < profundidadMaxima)
            {
                lineas.Add(lineaAristas);
                colores.Add(coloresAristas);
            }
        }
    }

    /// <summary>
    /// Dibuja en la línea de conectores la unión entre un nodo y sus hijos: ┌──┴──┐, ┌──┘ o └──┐.
    /// </summary>
    private static void DibujarConector(NodoBinario<T> nodo, Dictionary<NodoBinario<T>, int> centros,
        HashSet<NodoBinario<T>> marcados, char[] linea, int[] colores)
    {
        int centro = centros[nodo];
        bool izquierdoMarcado = nodo.Izquierdo != null && marcados.Contains(nodo.Izquierdo) && marcados.Contains(nodo);
        bool derechoMarcado = nodo.Derecho != null && marcados.Contains(nodo.Derecho) && marcados.Contains(nodo);

        if (nodo.Izquierdo != null)
        {
            int centroIzquierdo = centros[nodo.Izquierdo];
            linea[centroIzquierdo] = '┌';
            colores[centroIzquierdo] = izquierdoMarcado ? Marcado : Normal;
            for (int columna = centroIzquierdo + 1; columna < centro; columna++)
            {
                linea[columna] = '─';
                colores[columna] = izquierdoMarcado ? Marcado : Normal;
            }
        }

        if (nodo.Derecho != null)
        {
            int centroDerecho = centros[nodo.Derecho];
            linea[centroDerecho] = '┐';
            colores[centroDerecho] = derechoMarcado ? Marcado : Normal;
            for (int columna = centro + 1; columna < centroDerecho; columna++)
            {
                linea[columna] = '─';
                colores[columna] = derechoMarcado ? Marcado : Normal;
            }
        }

        if (nodo.Izquierdo != null && nodo.Derecho != null)
        {
            linea[centro] = '┴';
        }
        else if (nodo.Izquierdo != null)
        {
            linea[centro] = '┘';
        }
        else if (nodo.Derecho != null)
        {
            linea[centro] = '└';
        }

        colores[centro] = izquierdoMarcado || derechoMarcado ? Marcado : Normal;
    }

    /// <summary>
    /// Recorre el árbol en inorden y guarda el orden de los nodos y la profundidad de cada uno.
    /// </summary>
    private static void RecorrerInorden(NodoBinario<T>? nodo, int profundidad, List<NodoBinario<T>> inorden,
        Dictionary<NodoBinario<T>, int> profundidades)
    {
        if (nodo == null)
        {
            return;
        }

        RecorrerInorden(nodo.Izquierdo, profundidad + 1, inorden, profundidades);
        inorden.Add(nodo);
        profundidades[nodo] = profundidad;
        RecorrerInorden(nodo.Derecho, profundidad + 1, inorden, profundidades);
    }

    /// <summary>
    /// Construye la etiqueta de un nodo: [X] normal, (X) recorrido o &lt;X&gt; actual.
    /// </summary>
    private static string EtiquetaDe(NodoBinario<T> nodo, int color)
    {
        if (color == Actual)
        {
            return "<" + nodo.Valor + ">";
        }

        if (color == Marcado)
        {
            return "(" + nodo.Valor + ")";
        }

        return "[" + nodo.Valor + "]";
    }

    /// <summary>
    /// Decide cómo se resalta un nodo: actual, recorrido o normal.
    /// </summary>
    private static int ColorDe(NodoBinario<T> nodo, HashSet<NodoBinario<T>> marcados, NodoBinario<T>? actual)
    {
        if (nodo == actual)
        {
            return Actual;
        }

        if (marcados.Contains(nodo))
        {
            return Marcado;
        }

        return Normal;
    }

    /// <summary>
    /// Crea una línea de dibujo llena de espacios.
    /// </summary>
    /// <param name="ancho">Cantidad de columnas.</param>
    private static char[] NuevaLinea(int ancho)
    {
        char[] linea = new char[ancho];
        for (int columna = 0; columna < ancho; columna++)
        {
            linea[columna] = ' ';
        }

        return linea;
    }

    /// <summary>
    /// Escribe una línea en la consola, cambiando de color en cada tramo que tiene el mismo color.
    /// </summary>
    private static void EscribirConColor(char[] linea, int[] colores)
    {
        int longitud = new string(linea).TrimEnd().Length;
        int inicio = 0;
        while (inicio < longitud)
        {
            int fin = inicio;
            while (fin < longitud && colores[fin] == colores[inicio])
            {
                fin++;
            }

            if (colores[inicio] == Actual)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (colores[inicio] == Marcado)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            Console.Write(new string(linea, inicio, fin - inicio));
            Console.ResetColor();
            inicio = fin;
        }

        Console.WriteLine();
    }
}
