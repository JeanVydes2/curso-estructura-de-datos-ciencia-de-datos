using System.Globalization;

namespace TarifasExpresion;

/// <summary>
/// Construye el árbol de expresión de una fórmula escrita en notación sufija (postfija).
/// </summary>
public static class ConstructorDeExpresion
{
    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    /// <summary>
    /// Construye el árbol de una expresión en notación sufija.
    /// </summary>
    /// <remarks>
    /// Recorre los tokens con una pila: un número se apila como hoja y un operador desapila
    /// sus dos operandos, forma el nodo y lo apila. Al final la pila debe tener un solo árbol.
    /// </remarks>
    /// <param name="notacion">Expresión en sufijo, con los tokens separados por espacios.</param>
    /// <returns>La raíz del árbol de expresión.</returns>
    /// <exception cref="InvalidOperationException">Si la notación no describe un árbol de expresión.</exception>
    public static NodoExpresion Construir(string notacion)
    {
        Stack<NodoExpresion> pila = new Stack<NodoExpresion>();
        string[] tokens = notacion.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            throw new InvalidOperationException("La fórmula está vacía.");
        }

        foreach (string token in tokens)
        {
            if (EsNumero(token, out double valor))
            {
                pila.Push(new NodoExpresion(token, valor));
                continue;
            }

            if (!EsOperador(token))
            {
                throw new InvalidOperationException("El token " + token + " no es un número ni un operador.");
            }

            if (pila.Count < 2)
            {
                throw new InvalidOperationException("El operador " + token + " no tiene dos operandos.");
            }

            // El primero que sale de la pila es el operando derecho.
            NodoExpresion derecho = pila.Pop();
            NodoExpresion izquierdo = pila.Pop();
            pila.Push(new NodoExpresion(token, izquierdo, derecho));
        }

        if (pila.Count == 0)
        {
            throw new InvalidOperationException("La fórmula no produce ningún resultado.");
        }

        NodoExpresion raiz = pila.Pop();
        if (pila.Count > 0)
        {
            throw new InvalidOperationException("La fórmula deja " + pila.Count + " operandos sin operador.");
        }

        return raiz;
    }

    /// <summary>
    /// Indica si un token es uno de los cuatro operadores del caso.
    /// </summary>
    /// <param name="token">Token de la notación.</param>
    /// <returns><c>true</c> si el token es +, −, * o /.</returns>
    public static bool EsOperador(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    /// <summary>
    /// Indica si un token es un número y devuelve su valor.
    /// </summary>
    /// <param name="token">Token de la notación.</param>
    /// <param name="valor">Valor del número, cuando el token es un número.</param>
    /// <returns><c>true</c> si el token es un número.</returns>
    private static bool EsNumero(string token, out double valor)
    {
        return double.TryParse(token, NumberStyles.Float, Invariante, out valor);
    }
}
