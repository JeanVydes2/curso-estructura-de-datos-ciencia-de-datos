namespace RotacionFefo.Models;

/// <summary>
/// Un lote de producto perecedero que espera en la bodega. DaysToExpire son los días que le
/// quedan antes de vencer y ArrivalOrder registra el orden en que llegó a la bodega; se usa
/// para desempatar cuando dos lotes vencen el mismo día.
/// </summary>
public class Lote
{
    public string Code { get; }
    public string Product { get; }
    public int DaysToExpire { get; }
    public int ArrivalOrder { get; }

    /// <summary>
    /// Crea un lote de producto perecedero.
    /// </summary>
    /// <param name="code">Identificador del lote.</param>
    /// <param name="product">Nombre del producto.</param>
    /// <param name="daysToExpire">Días que le quedan al lote antes de vencer.</param>
    /// <param name="arrivalOrder">Orden de llegada a la bodega; se usa para desempatar.</param>
    public Lote(string code, string product, int daysToExpire, int arrivalOrder)
    {
        Code = code;
        Product = product;
        DaysToExpire = daysToExpire;
        ArrivalOrder = arrivalOrder;
    }

    /// <summary>
    /// Texto que usa el dibujo del montículo: código y días restantes.
    /// </summary>
    public override string ToString()
    {
        return Code + ":" + DaysToExpire;
    }
}
