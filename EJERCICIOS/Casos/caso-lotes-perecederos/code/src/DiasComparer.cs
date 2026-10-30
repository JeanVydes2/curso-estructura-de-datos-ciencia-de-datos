using RotacionFefo.Models;

namespace RotacionFefo;

/// <summary>
/// Compara dos lotes solo por los días que les quedan antes de vencer, sin desempatar por llegada.
/// Se usa en la comparación del inciso 10 para mostrar qué cambia cuando no hay desempate.
/// </summary>
public class DiasComparer : IComparer<Lote>
{
    /// <summary>
    /// Compara dos lotes por sus días restantes. Si empatan, devuelve cero.
    /// </summary>
    public int Compare(Lote? x, Lote? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        return x.DaysToExpire.CompareTo(y.DaysToExpire);
    }
}
