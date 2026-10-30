namespace DataQualityIndex;

public sealed class BinaryNode<T>
{
    /// <summary>Dato almacenado en el nodo.</summary>
    public T Value { get; set; }

    /// <summary>Hijo con claves menores que la clave del nodo.</summary>
    public BinaryNode<T>? Left { get; set; }

    /// <summary>Hijo con claves mayores que la clave del nodo.</summary>
    public BinaryNode<T>? Right { get; set; }

    public BinaryNode(T value)
    {
        Value = value;
    }
}
