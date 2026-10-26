namespace IndiceDePrecios;

/// <summary>
/// Dibuja el árbol de precios de arriba hacia abajo en la consola.
/// Cada nodo ocupa una columna según su posición en inorden, y su etiqueta es
/// <c>[precio:tamaño]</c>, para poder revisar el tamaño de cada subárbol.
/// </summary>
public static class ImpresorPrecios
{
    private const int Separacion = 2;

    /// <summary>
    /// Dibuja el árbol en la consola, resaltando un nodo actual entre &lt; &gt;.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    /// <param name="actual">Nodo que se resalta; puede ser <c>null</c>.</param>
    public static void Imprimir(NodoPrecio? raiz, NodoPrecio? actual)
    {
        List<string> lineas = RenderizarSimple(raiz);
        foreach (string linea in lineas)
        {
            EscribirResaltado(linea, raiz, actual);
        }
    }

    /// <summary>
    /// Dibuja el árbol en la consola sin resaltar ningún nodo.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    public static void Imprimir(NodoPrecio? raiz)
    {
        Imprimir(raiz, null);
    }

    /// <summary>
    /// Genera el dibujo del árbol como texto, sin colores ni marcas.
    /// </summary>
    /// <param name="raiz">Raíz del árbol.</param>
    /// <returns>Una línea de texto por cada fila del dibujo. Se usa también en las pruebas.</returns>
    public static List<string> RenderizarSimple(NodoPrecio? raiz)
    {
        List<string> resultado = new List<string>();
        if (raiz == null)
        {
            resultado.Add("(árbol vacío)");
            return resultado;
        }

        Dictionary<NodoPrecio, int> centros = new Dictionary<NodoPrecio, int>();
        Dictionary<NodoPrecio, int> profundidades = new Dictionary<NodoPrecio, int>();
        List<NodoPrecio> inorden = new List<NodoPrecio>();
        RecorrerInorden(raiz, 0, inorden, profundidades);

        int ancho = 0;
        int profundidadMaxima = 0;
        foreach (NodoPrecio nodo in inorden)
        {
            string etiqueta = EtiquetaDe(nodo);
            centros[nodo] = ancho + etiqueta.Length / 2;
            ancho += etiqueta.Length + Separacion;
            if (profundidades[nodo] > profundidadMaxima)
            {
                profundidadMaxima = profundidades[nodo];
            }
        }

        for (int profundidad = 0; profundidad <= profundidadMaxima; profundidad++)
        {
            char[] lineaNodos = NuevaLinea(ancho);
            char[] lineaAristas = NuevaLinea(ancho);

            foreach (NodoPrecio nodo in inorden)
            {
                if (profundidades[nodo] != profundidad)
                {
                    continue;
                }

                string etiqueta = EtiquetaDe(nodo);
                int inicio = centros[nodo] - etiqueta.Length / 2;
                for (int desplazamiento = 0; desplazamiento < etiqueta.Length; desplazamiento++)
                {
                    lineaNodos[inicio + desplazamiento] = etiqueta[desplazamiento];
                }

                DibujarConector(nodo, centros, lineaAristas);
            }

            resultado.Add(new string(lineaNodos).TrimEnd());
            if (profundidad < profundidadMaxima)
            {
                resultado.Add(new string(lineaAristas).TrimEnd());
            }
        }

        return resultado;
    }

    /// <summary>
    /// Construye la etiqueta de un nodo: [precio:tamaño].
    /// </summary>
    private static string EtiquetaDe(NodoPrecio nodo)
    {
        return "[" + nodo.Precio + ":" + nodo.Tamano + "]";
    }

    /// <summary>
    /// Dibuja la unión entre un nodo y sus hijos: ┌──┴──┐, ┌──┘ o └──┐.
    /// </summary>
    private static void DibujarConector(NodoPrecio nodo, Dictionary<NodoPrecio, int> centros, char[] linea)
    {
        int centro = centros[nodo];

        if (nodo.Izquierdo != null)
        {
            int centroIzquierdo = centros[nodo.Izquierdo];
            linea[centroIzquierdo] = '┌';
            for (int columna = centroIzquierdo + 1; columna < centro; columna++)
            {
                linea[columna] = '─';
            }
        }

        if (nodo.Derecho != null)
        {
            int centroDerecho = centros[nodo.Derecho];
            linea[centroDerecho] = '┐';
            for (int columna = centro + 1; columna < centroDerecho; columna++)
            {
                linea[columna] = '─';
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
    }

    /// <summary>
    /// Recorre el árbol en inorden y guarda el orden de los nodos y la profundidad de cada uno.
    /// </summary>
    private static void RecorrerInorden(NodoPrecio? nodo, int profundidad, List<NodoPrecio> inorden,
        Dictionary<NodoPrecio, int> profundidades)
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
    /// Crea una línea de dibujo llena de espacios.
    /// </summary>
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
    /// Escribe una línea y colorea en amarillo la etiqueta del nodo actual.
    /// </summary>
    private static void EscribirResaltado(string linea, NodoPrecio? raiz, NodoPrecio? actual)
    {
        if (actual == null)
        {
            Console.WriteLine(linea);
            return;
        }

        string etiqueta = "[" + actual.Precio + ":" + actual.Tamano + "]";
        int posicion = linea.IndexOf(etiqueta, StringComparison.Ordinal);
        if (posicion < 0)
        {
            Console.WriteLine(linea);
            return;
        }

        Console.Write(linea.Substring(0, posicion));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("<" + actual.Precio + ":" + actual.Tamano + ">");
        Console.ResetColor();
        Console.WriteLine(linea.Substring(posicion + etiqueta.Length));
    }
}
