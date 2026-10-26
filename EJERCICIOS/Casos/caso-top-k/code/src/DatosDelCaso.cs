using LluviaTopK.Modelos;

namespace LluviaTopK;

/// <summary>
/// Los datos del caso, en el mismo orden en que aparecen en caso.md.
/// </summary>
public static class DatosDelCaso
{
    /// <summary>
    /// Devuelve el lote de prueba con los acumulados de los 12 pluviómetros.
    /// </summary>
    /// <returns>Las lecturas del lote, en el orden en que llegaron.</returns>
    public static List<Acumulado> Lote()
    {
        List<Acumulado> lote = new List<Acumulado>();
        lote.Add(new Acumulado("P01", 12.4));
        lote.Add(new Acumulado("P02", 0.0));
        lote.Add(new Acumulado("P03", 35.8));
        lote.Add(new Acumulado("P04", 7.2));
        lote.Add(new Acumulado("P05", 51.3));
        lote.Add(new Acumulado("P06", 4.6));
        lote.Add(new Acumulado("P07", 28.9));
        lote.Add(new Acumulado("P08", 0.3));
        lote.Add(new Acumulado("P09", 65.7));
        lote.Add(new Acumulado("P10", 18.1));
        lote.Add(new Acumulado("P11", 9.5));
        lote.Add(new Acumulado("P12", 42.0));
        return lote;
    }

    /// <summary>
    /// Devuelve el valor de K del boletín.
    /// </summary>
    /// <returns>Cantidad de puntos que pide el boletín.</returns>
    public static int K()
    {
        return 3;
    }
}
