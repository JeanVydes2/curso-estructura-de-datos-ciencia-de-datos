namespace TriajeUrgencias;

/// <summary>
/// Dibuja el montículo de dos formas: como árbol y como la lista que realmente lo guarda.
/// En el árbol, los hijos de la posición i son 2i + 1 y 2i + 2. El elemento resaltado se
/// muestra entre &lt; &gt; y en amarillo, para que se vea aunque la consola no tenga colores.
/// </summary>
public static class ImpresorMonticulo
{
    private const int Separacion = 2;

    /// <summary>
    /// Dibuja el montículo como árbol y debajo la lista con sus índices.
    /// </summary>
    /// <param name="elementos">Lista del montículo, en el orden en que está guardada.</param>
    /// <param name="indiceResaltado">Posición que se resalta con &lt; &gt; y en amarillo; -1 para no resaltar.</param>
    public static void Imprimir<T>(List<T> elementos, int indiceResaltado)
    {
        List<string> lineas = RenderizarArbol(elementos, indiceResaltado);
        foreach (string linea in lineas)
        {
            EscribirResaltado(linea);
        }

        Console.WriteLine();
        Console.Write("items: ");
        for (int indice = 0; indice < elementos.Count; indice++)
        {
            string celda = "[" + indice + "] " + elementos[indice];
            if (indice == indiceResaltado)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("<" + celda + ">");
                Console.ResetColor();
            }
            else
            {
                Console.Write(celda);
            }

            Console.Write("  ");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Genera el dibujo del árbol como texto. Los hijos de la posición i están en 2i + 1 y 2i + 2.
    /// </summary>
    /// <param name="elementos">Lista del montículo.</param>
    /// <param name="indiceResaltado">Posición que se resalta; -1 para no resaltar.</param>
    /// <returns>Una línea de texto por cada fila del dibujo.</returns>
    public static List<string> RenderizarArbol<T>(List<T> elementos, int indiceResaltado)
    {
        List<string> resultado = new List<string>();
        if (elementos.Count == 0)
        {
            resultado.Add("(montículo vacío)");
            return resultado;
        }

        List<int> inorden = new List<int>();
        RecorrerInorden(0, elementos.Count, inorden);

        int[] centros = new int[elementos.Count];
        int ancho = 0;
        foreach (int indice in inorden)
        {
            string etiqueta = EtiquetaDe(elementos, indice, indiceResaltado);
            centros[indice] = ancho + etiqueta.Length / 2;
            ancho += etiqueta.Length + Separacion;
        }

        int profundidadMaxima = ProfundidadDe(elementos.Count - 1);
        for (int profundidad = 0; profundidad <= profundidadMaxima; profundidad++)
        {
            char[] lineaNodos = NuevaLinea(ancho);
            char[] lineaAristas = NuevaLinea(ancho);

            for (int indice = 0; indice < elementos.Count; indice++)
            {
                if (ProfundidadDe(indice) != profundidad)
                {
                    continue;
                }

                string etiqueta = EtiquetaDe(elementos, indice, indiceResaltado);
                int inicio = centros[indice] - etiqueta.Length / 2;
                for (int desplazamiento = 0; desplazamiento < etiqueta.Length; desplazamiento++)
                {
                    lineaNodos[inicio + desplazamiento] = etiqueta[desplazamiento];
                }

                DibujarConector(indice, elementos.Count, centros, lineaAristas);
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
    /// Dibuja la unión entre la posición <paramref name="indice"/> y sus hijos.
    /// </summary>
    private static void DibujarConector(int indice, int cantidad, int[] centros, char[] linea)
    {
        int izquierdo = 2 * indice + 1;
        int derecho = 2 * indice + 2;
        int centro = centros[indice];

        if (izquierdo < cantidad)
        {
            linea[centros[izquierdo]] = '┌';
            for (int columna = centros[izquierdo] + 1; columna < centro; columna++)
            {
                linea[columna] = '─';
            }
        }

        if (derecho < cantidad)
        {
            linea[centros[derecho]] = '┐';
            for (int columna = centro + 1; columna < centros[derecho]; columna++)
            {
                linea[columna] = '─';
            }
        }

        if (izquierdo < cantidad && derecho < cantidad)
        {
            linea[centro] = '┴';
        }
        else if (izquierdo < cantidad)
        {
            linea[centro] = '┘';
        }
    }

    /// <summary>
    /// Recorre las posiciones en inorden para asignar a cada una su columna en el dibujo.
    /// </summary>
    private static void RecorrerInorden(int indice, int cantidad, List<int> inorden)
    {
        if (indice >= cantidad)
        {
            return;
        }

        RecorrerInorden(2 * indice + 1, cantidad, inorden);
        inorden.Add(indice);
        RecorrerInorden(2 * indice + 2, cantidad, inorden);
    }

    /// <summary>
    /// Calcula el nivel de una posición: ⌊log₂ (indice + 1)⌋.
    /// </summary>
    private static int ProfundidadDe(int indice)
    {
        int profundidad = 0;
        int posicion = indice + 1;
        while (posicion > 1)
        {
            posicion = posicion / 2;
            profundidad++;
        }

        return profundidad;
    }

    /// <summary>
    /// Construye la etiqueta de una posición: [X] normal o &lt;X&gt; resaltada.
    /// </summary>
    private static string EtiquetaDe<T>(List<T> elementos, int indice, int indiceResaltado)
    {
        if (indice == indiceResaltado)
        {
            return "<" + elementos[indice] + ">";
        }

        return "[" + elementos[indice] + "]";
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
    /// Escribe una línea y colorea en amarillo el tramo entre &lt; y &gt;.
    /// </summary>
    private static void EscribirResaltado(string linea)
    {
        int apertura = linea.IndexOf('<');
        int cierre = linea.IndexOf('>');
        if (apertura < 0 || cierre < apertura)
        {
            Console.WriteLine(linea);
            return;
        }

        Console.Write(linea.Substring(0, apertura));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(linea.Substring(apertura, cierre - apertura + 1));
        Console.ResetColor();
        Console.WriteLine(linea.Substring(cierre + 1));
    }
}
