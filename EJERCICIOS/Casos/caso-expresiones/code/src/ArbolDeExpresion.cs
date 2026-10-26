using System.Globalization;

namespace TarifasExpresion;

/// <summary>
/// Calcula y escribe una fórmula recorriendo su árbol: valor en postorden, texto en
/// prefijo, infijo con paréntesis y sufijo, y métricas de forma.
/// </summary>
public static class ArbolDeExpresion
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-CO");

    /// <summary>
    /// Calcula el valor de la expresión recorriendo el árbol en postorden.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>El valor de la fórmula.</returns>
    /// <exception cref="InvalidOperationException">Si una operación divide por cero.</exception>
    public static double Evaluar(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            throw new InvalidOperationException("No hay fórmula que evaluar.");
        }

        if (raiz.EsHoja)
        {
            return raiz.Valor;
        }

        double izquierdo = Evaluar(raiz.Izquierdo);
        double derecho = Evaluar(raiz.Derecho);

        switch (raiz.Texto)
        {
            case "+":
                return izquierdo + derecho;
            case "-":
                return izquierdo - derecho;
            case "*":
                return izquierdo * derecho;
            case "/":
                if (derecho == 0)
                {
                    throw new InvalidOperationException("La fórmula divide por cero.");
                }

                return izquierdo / derecho;
            default:
                throw new InvalidOperationException("El token " + raiz.Texto + " no es un operador.");
        }
    }

    /// <summary>
    /// Escribe la expresión en prefijo, recorriendo el árbol en preorden.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>La expresión en notación prefija.</returns>
    public static string Prefijo(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return "";
        }

        if (raiz.EsHoja)
        {
            return raiz.Texto;
        }

        return raiz.Texto + " " + Prefijo(raiz.Izquierdo) + " " + Prefijo(raiz.Derecho);
    }

    /// <summary>
    /// Escribe la expresión en sufijo, recorriendo el árbol en postorden.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>La expresión en notación sufija.</returns>
    public static string Sufijo(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return "";
        }

        if (raiz.EsHoja)
        {
            return raiz.Texto;
        }

        return Sufijo(raiz.Izquierdo) + " " + Sufijo(raiz.Derecho) + " " + raiz.Texto;
    }

    /// <summary>
    /// Escribe la expresión en infijo, con paréntesis alrededor de cada operación.
    /// </summary>
    /// <remarks>
    /// Sin paréntesis, el infijo no conserva la forma del árbol y se puede leer con otro resultado.
    /// </remarks>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>La expresión en notación infija.</returns>
    public static string Infijo(NodoExpresion? raiz)
    {
        return InfijoConParentesis(raiz, false);
    }

    /// <summary>
    /// Cuenta los nodos del árbol.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Número de nodos; 0 si el árbol está vacío.</returns>
    public static int ContarNodos(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return 0;
        }

        return 1 + ContarNodos(raiz.Izquierdo) + ContarNodos(raiz.Derecho);
    }

    /// <summary>
    /// Cuenta las hojas, que son los números de la fórmula.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Número de hojas.</returns>
    public static int ContarHojas(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return 0;
        }

        if (raiz.EsHoja)
        {
            return 1;
        }

        return ContarHojas(raiz.Izquierdo) + ContarHojas(raiz.Derecho);
    }

    /// <summary>
    /// Cuenta los operadores, que son los nodos internos de la fórmula.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Número de operadores.</returns>
    public static int ContarOperadores(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return 0;
        }

        if (raiz.EsHoja)
        {
            return 0;
        }

        return 1 + ContarOperadores(raiz.Izquierdo) + ContarOperadores(raiz.Derecho);
    }

    /// <summary>
    /// Calcula la altura del árbol, medida en aristas.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Aristas del camino más largo; -1 si el árbol está vacío.</returns>
    public static int Altura(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return -1;
        }

        int alturaIzquierda = Altura(raiz.Izquierdo);
        int alturaDerecha = Altura(raiz.Derecho);
        return 1 + Math.Max(alturaIzquierda, alturaDerecha);
    }

    /// <summary>
    /// Calcula el ancho: la mayor cantidad de nodos que hay en un mismo nivel.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Nodos del nivel más poblado; 0 si el árbol está vacío.</returns>
    public static int Ancho(NodoExpresion? raiz)
    {
        if (raiz == null)
        {
            return 0;
        }

        int anchoMaximo = 0;
        Queue<NodoExpresion> pendientes = new Queue<NodoExpresion>();
        pendientes.Enqueue(raiz);

        while (pendientes.Count > 0)
        {
            int nodosDelNivel = pendientes.Count;
            if (nodosDelNivel > anchoMaximo)
            {
                anchoMaximo = nodosDelNivel;
            }

            for (int indice = 0; indice < nodosDelNivel; indice++)
            {
                NodoExpresion nodo = pendientes.Dequeue();
                if (nodo.Izquierdo != null)
                {
                    pendientes.Enqueue(nodo.Izquierdo);
                }

                if (nodo.Derecho != null)
                {
                    pendientes.Enqueue(nodo.Derecho);
                }
            }
        }

        return anchoMaximo;
    }

    /// <summary>
    /// Agrupa los nodos por nivel, desde la raíz hacia abajo.
    /// </summary>
    /// <param name="raiz">Raíz del árbol de expresión.</param>
    /// <returns>Una lista de nodos por nivel.</returns>
    public static List<List<NodoExpresion>> Niveles(NodoExpresion? raiz)
    {
        List<List<NodoExpresion>> niveles = new List<List<NodoExpresion>>();
        if (raiz == null)
        {
            return niveles;
        }

        Queue<NodoExpresion> pendientes = new Queue<NodoExpresion>();
        pendientes.Enqueue(raiz);

        while (pendientes.Count > 0)
        {
            int nodosDelNivel = pendientes.Count;
            List<NodoExpresion> nivel = new List<NodoExpresion>();

            for (int indice = 0; indice < nodosDelNivel; indice++)
            {
                NodoExpresion nodo = pendientes.Dequeue();
                nivel.Add(nodo);

                if (nodo.Izquierdo != null)
                {
                    pendientes.Enqueue(nodo.Izquierdo);
                }

                if (nodo.Derecho != null)
                {
                    pendientes.Enqueue(nodo.Derecho);
                }
            }

            niveles.Add(nivel);
        }

        return niveles;
    }

    /// <summary>
    /// Da formato decimal a un valor, con coma y sin ceros sobrantes.
    /// </summary>
    /// <param name="valor">Valor que se escribe.</param>
    /// <returns>El valor en texto, por ejemplo "16".</returns>
    public static string Formato(double valor)
    {
        return valor.ToString("0.###", Espanol);
    }

    /// <summary>
    /// Escribe el infijo y agrega paréntesis a cada subárbol que sea una operación.
    /// </summary>
    private static string InfijoConParentesis(NodoExpresion? nodo, bool entreParentesis)
    {
        if (nodo == null)
        {
            return "";
        }

        if (nodo.EsHoja)
        {
            return nodo.Texto;
        }

        string cuerpo = InfijoConParentesis(nodo.Izquierdo, true) + " " + nodo.Texto + " "
            + InfijoConParentesis(nodo.Derecho, true);
        return entreParentesis ? "(" + cuerpo + ")" : cuerpo;
    }
}
