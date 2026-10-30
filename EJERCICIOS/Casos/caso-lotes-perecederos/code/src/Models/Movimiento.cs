namespace RotacionFefo.Models;

/// <summary>
/// Un movimiento de la bodega: llega un lote nuevo o un retiro saca el lote más próximo a vencer.
/// Lot es null cuando el movimiento es un retiro.
/// </summary>
public class Movimiento
{
    public int Order { get; }
    public Lote? Lot { get; }

    /// <summary>
    /// Crea un movimiento. Se usa a través de Llegada y Retiro.
    /// </summary>
    public Movimiento(int order, Lote? lot)
    {
        Order = order;
        Lot = lot;
    }

    /// <summary>
    /// Crea el movimiento de llegada de un lote a la bodega.
    /// </summary>
    public static Movimiento Llegada(int order, Lote lote)
    {
        return new Movimiento(order, lote);
    }

    /// <summary>
    /// Crea el movimiento de retiro del lote más próximo a vencer.
    /// </summary>
    public static Movimiento Retiro(int order)
    {
        return new Movimiento(order, null);
    }

    /// <summary>
    /// Indica si el movimiento es una llegada.
    /// </summary>
    public bool EsLlegada
    {
        get
        {
            return Lot != null;
        }
    }
}
