using LluviaTopK.Modelos;

namespace LluviaTopK;

/// <summary>
/// Va primero el acumulado con menos milímetros. Si empatan, el de menor identificador,
/// para que el resultado no dependa del orden en que llegaron las lecturas.
/// </summary>
public class ComparadorPorMilimetros : IComparer<Acumulado>
{
    /// <summary>
    /// Compara dos acumulados: va primero el de menos milímetros y, si empatan, el de menor id.
    /// </summary>
    /// <param name="x">Primer acumulado.</param>
    /// <param name="y">Segundo acumulado.</param>
    /// <returns>Negativo si x va antes que y, cero si son equivalentes y positivo si va después.</returns>
    public int Compare(Acumulado? x, Acumulado? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        int resultado = x.Milimetros.CompareTo(y.Milimetros);
        if (resultado != 0)
        {
            return resultado;
        }

        return string.CompareOrdinal(x.Id, y.Id);
    }
}
