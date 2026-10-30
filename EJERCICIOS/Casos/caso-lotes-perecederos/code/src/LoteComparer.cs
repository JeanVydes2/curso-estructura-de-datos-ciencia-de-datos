using RotacionFefo.Models;

namespace RotacionFefo;

/// <summary>
/// Va primero el lote con menos días para vencer. Si empatan, el que llegó antes a la bodega.
/// El desempate está en el comparador porque el montículo, por sí solo, no conserva el orden de llegada.
/// </summary>
public class LoteComparer : IComparer<Lote>
{
    /// <summary>
    /// Compara dos lotes: va primero el que vence antes y, si empatan, el que llegó antes.
    /// </summary>
    /// <returns>Negativo si x va antes que y, cero si son equivalentes y positivo si va después.</returns>
    public int Compare(Lote? x, Lote? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        int result = x.DaysToExpire.CompareTo(y.DaysToExpire);
        if (result != 0)
        {
            return result;
        }

        return x.ArrivalOrder.CompareTo(y.ArrivalOrder);
    }
}
