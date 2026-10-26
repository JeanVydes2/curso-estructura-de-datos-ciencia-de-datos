using CatalogoEstaciones.Modelos;

namespace CatalogoEstaciones;

/// <summary>
/// Ordena estaciones por código; se usa para simular un inventario exportado ya ordenado.
/// </summary>
public class ComparadorDeCodigo : IComparer<Estacion>
{
    /// <summary>
    /// Compara dos estaciones por código.
    /// </summary>
    /// <param name="x">Primera estación.</param>
    /// <param name="y">Segunda estación.</param>
    /// <returns>Negativo si x va antes que y, cero si tienen el mismo código y positivo si va después.</returns>
    public int Compare(Estacion? x, Estacion? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        return x.Codigo.CompareTo(y.Codigo);
    }
}
