namespace TarifasExpresion;

/// <summary>
/// Un nodo del árbol de expresión. Un nodo hoja guarda un número; un nodo operador
/// guarda el signo y sus dos hijos. En un operador, Izquierdo y Derecho nunca son <c>null</c>.
/// </summary>
public class NodoExpresion
{
    /// <summary>
    /// Texto del nodo: un número, por ejemplo "2.5", o un signo, por ejemplo "+".
    /// </summary>
    public string Texto { get; }

    /// <summary>
    /// Hijo izquierdo; <c>null</c> si el nodo guarda un número.
    /// </summary>
    public NodoExpresion? Izquierdo { get; }

    /// <summary>
    /// Hijo derecho; <c>null</c> si el nodo guarda un número.
    /// </summary>
    public NodoExpresion? Derecho { get; }

    /// <summary>
    /// Crea un nodo hoja que guarda un número.
    /// </summary>
    /// <param name="texto">Número de la fórmula, como se escribió.</param>
    /// <param name="valor">Valor numérico del número.</param>
    public NodoExpresion(string texto, double valor)
    {
        Texto = texto;
        Valor = valor;
        Izquierdo = null;
        Derecho = null;
    }

    /// <summary>
    /// Crea un nodo operador con sus dos operandos.
    /// </summary>
    /// <param name="texto">Signo de la operación.</param>
    /// <param name="izquierdo">Operando izquierdo.</param>
    /// <param name="derecho">Operando derecho.</param>
    public NodoExpresion(string texto, NodoExpresion izquierdo, NodoExpresion derecho)
    {
        Texto = texto;
        Valor = 0;
        Izquierdo = izquierdo;
        Derecho = derecho;
    }

    /// <summary>
    /// Valor del nodo cuando es un número; 0 cuando es un operador.
    /// </summary>
    public double Valor { get; }

    /// <summary>
    /// Indica si el nodo guarda un número y por lo tanto no tiene hijos.
    /// </summary>
    public bool EsHoja
    {
        get
        {
            return Izquierdo == null && Derecho == null;
        }
    }

    /// <summary>
    /// Devuelve el texto del nodo, que es la etiqueta que se dibuja en el árbol.
    /// </summary>
    /// <returns>El número o el signo que guarda el nodo.</returns>
    public override string ToString()
    {
        return Texto;
    }
}
