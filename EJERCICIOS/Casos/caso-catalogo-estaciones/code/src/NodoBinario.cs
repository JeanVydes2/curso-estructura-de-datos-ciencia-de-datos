namespace CatalogoEstaciones;

/// <summary>
/// Un nodo guarda un dato y dos referencias: Izquierdo y Derecho son las aristas hacia sus hijos.
/// </summary>
/// <typeparam name="T">Tipo del dato que guarda el nodo.</typeparam>
public class NodoBinario<T>
{
    /// <summary>
    /// Dato que guarda el nodo.
    /// </summary>
    public T Valor { get; set; }

    /// <summary>
    /// Referencia al hijo izquierdo; <c>null</c> si no tiene. Es la arista hacia ese hijo.
    /// </summary>
    public NodoBinario<T>? Izquierdo { get; set; }

    /// <summary>
    /// Referencia al hijo derecho; <c>null</c> si no tiene. Es la arista hacia ese hijo.
    /// </summary>
    public NodoBinario<T>? Derecho { get; set; }

    /// <summary>
    /// Crea un nodo sin hijos: Izquierdo y Derecho empiezan en <c>null</c>.
    /// </summary>
    /// <param name="valor">Dato que guarda el nodo.</param>
    public NodoBinario(T valor)
    {
        Valor = valor;
        Izquierdo = null;
        Derecho = null;
    }

    /// <summary>
    /// Indica si el nodo es una hoja, es decir, si no tiene hijo izquierdo ni derecho.
    /// </summary>
    public bool EsHoja
    {
        get
        {
            return Izquierdo == null && Derecho == null;
        }
    }
}
