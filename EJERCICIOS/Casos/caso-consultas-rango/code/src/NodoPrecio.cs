namespace IndiceDePrecios;

/// <summary>
/// Un nodo del índice de precios. Guarda el precio, sus dos hijos y <see cref="Tamano"/>,
/// el número de nodos que cuelgan de él, incluyéndolo.
/// </summary>
public class NodoPrecio
{
    /// <summary>
    /// Precio que guarda el nodo, en pesos por kilogramo.
    /// </summary>
    public int Precio { get; set; }

    /// <summary>
    /// Producto al que corresponde el precio.
    /// </summary>
    public string Producto { get; set; }

    /// <summary>
    /// Hijo izquierdo; <c>null</c> si no tiene. Guarda precios menores.
    /// </summary>
    public NodoPrecio? Izquierdo { get; set; }

    /// <summary>
    /// Hijo derecho; <c>null</c> si no tiene. Guarda precios mayores.
    /// </summary>
    public NodoPrecio? Derecho { get; set; }

    /// <summary>
    /// Número de nodos del subárbol que empieza en este nodo, incluyéndolo.
    /// </summary>
    /// <remarks>
    /// Es 1 para una hoja y 1 + tamaño del izquierdo + tamaño del derecho para un nodo interno.
    /// Permite que el k-ésimo y el conteo por debajo no recorran todo el árbol.
    /// </remarks>
    public int Tamano { get; set; }

    /// <summary>
    /// Crea un nodo sin hijos, con tamaño 1.
    /// </summary>
    /// <param name="precio">Precio que guarda el nodo.</param>
    /// <param name="producto">Producto al que corresponde.</param>
    public NodoPrecio(int precio, string producto)
    {
        Precio = precio;
        Producto = producto;
        Izquierdo = null;
        Derecho = null;
        Tamano = 1;
    }

    /// <summary>
    /// Devuelve el precio, que es la clave y la etiqueta que se dibuja en el árbol.
    /// </summary>
    /// <returns>El precio del nodo.</returns>
    public override string ToString()
    {
        return Precio.ToString();
    }
}
