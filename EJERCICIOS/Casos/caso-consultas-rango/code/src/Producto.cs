namespace IndiceDePrecios;

/// <summary>
/// Datos de un producto de la lonja: nombre y precio por kilogramo.
/// </summary>
/// <param name="Nombre">Nombre del producto.</param>
/// <param name="Precio">Precio por kilogramo, en pesos.</param>
public record Producto(string Nombre, int Precio);
