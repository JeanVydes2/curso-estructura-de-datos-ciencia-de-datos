using System.Text;

namespace TarifasExpresion;

/// <summary>
/// Dibuja un árbol de expresión de arriba hacia abajo en la consola.
/// Cada nodo ocupa una columna según su posición en inorden, así un hijo izquierdo
/// siempre queda a la izquierda de su padre y uno derecho, a la derecha.
/// </summary>
public static class ImpresorExpresion
{
    private const int Separacion = 2;

    /// <summary>
    /// Dibuja el árbol en la consola, resaltando un nodo actual en amarillo entre &lt; &gt;.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    /// <param name="actual">Nodo que se resalta; puede ser <c>null</c>.</param>
    public static void Imprimir(NodoExpresion? raiz, NodoExpresion? actual)
    {
        List<string> lineas = RenderizarSimple(raiz);
        foreach (string linea in lineas)
        {
            EscribirResaltado(linea, actual == null ? null : actual.Texto);
        }
    }

    /// <summary>
    /// Dibuja el árbol en la consola sin resaltar ningún nodo.
    /// </summary>
    /// <param name="raiz">Raíz del árbol que se dibuja.</param>
    public static void Imprimir(NodoExpresion? raiz)
    {
        Imprimir(raiz, null);
    }

    /// <summary>
    /// Genera el dibujo del árbol como texto, sin colores ni marcas.
    /// </summary>
    /// <param name="raiz">Raíz del árbol.</param>
    /// <returns>Una línea de texto por cada fila del dibujo. Se usa también en las pruebas.</returns>
    public static List<string> RenderizarSimple(NodoExpresion? raiz)
    {
        List<string> resultado = new List<string>();
        if (raiz == null)
        {
            resultado.Add("(árbol vacío)");
            return resultado;
        }

        Dictionary<NodoExpresion, int> centros = new Dictionary<NodoExpresion, int>();
        Dictionary<NodoExpresion, int> profundidades = new Dictionary<NodoExpresion, int>();
        List<NodoExpresion> inorden = new List<NodoExpresion>();
        RecorrerInorden(raiz, 0, inorden, profundidades);

        int ancho = 0;
        int profundidadMaxima = 0;
        foreach (NodoExpresion nodo in inorden)
        {
            string etiqueta = "[" + nodo.Texto + "]";
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

            foreach (NodoExpresion nodo in inorden)
            {
                if (profundidades[nodo] != profundidad)
                {
                    continue;
                }

                string etiqueta = "[" + nodo.Texto + "]";
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
    /// Reemplaza el texto de un nodo por &lt;texto&gt; para marcarlo en el dibujo resaltado.
    /// </summary>
    /// <param name="linea">Línea del dibujo.</param>
    /// <param name="etiqueta">Texto del nodo a resaltar, sin corchetes.</param>
    /// <returns>La línea con el nodo resaltado.</returns>
    private static string Marcar(string linea, string etiqueta)
    {
        return linea.Replace("[" + etiqueta + "]", "<" + etiqueta + ">");
    }

    /// <summary>
    /// Dibuja la unión entre un nodo y sus hijos: ┌──┴──┐, ┌──┘ o └──┐.
    /// </summary>
    private static void DibujarConector(NodoExpresion nodo, Dictionary<NodoExpresion, int> centros, char[] linea)
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
    private static void RecorrerInorden(NodoExpresion? nodo, int profundidad, List<NodoExpresion> inorden,
        Dictionary<NodoExpresion, int> profundidades)
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
    /// Escribe una línea y colorea en amarillo el tramo entre &lt; y &gt;.
    /// </summary>
    private static void EscribirResaltado(string linea, string? etiqueta)
    {
        string texto = etiqueta == null ? linea : Marcar(linea, etiqueta);
        int apertura = texto.IndexOf('<');
        int cierre = texto.IndexOf('>');
        if (apertura < 0 || cierre < apertura)
        {
            Console.WriteLine(texto);
            return;
        }

        Console.Write(texto.Substring(0, apertura));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(texto.Substring(apertura, cierre - apertura + 1));
        Console.ResetColor();
        Console.WriteLine(texto.Substring(cierre + 1));
    }
}
