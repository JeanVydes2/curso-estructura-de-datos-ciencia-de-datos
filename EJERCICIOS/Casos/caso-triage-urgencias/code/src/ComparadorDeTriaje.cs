using TriajeUrgencias.Modelos;

namespace TriajeUrgencias;

/// <summary>
/// Va primero el paciente con menor nivel de prioridad. Si empatan, el que llegó antes.
/// El desempate está en el comparador porque el montículo, por sí solo, no conserva el orden de llegada.
/// </summary>
public class ComparadorDeTriaje : IComparer<Paciente>
{
    /// <summary>
    /// Compara dos pacientes: va primero el de menor prioridad y, si empatan, el que llegó antes.
    /// </summary>
    /// <remarks>
    /// El desempate está aquí porque el montículo, por sí solo, no conserva el orden de llegada.
    /// </remarks>
    /// <param name="x">Primer paciente.</param>
    /// <param name="y">Segundo paciente.</param>
    /// <returns>Negativo si x va antes que y, cero si son equivalentes y positivo si va después.</returns>
    public int Compare(Paciente? x, Paciente? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        int resultado = x.Prioridad.CompareTo(y.Prioridad);
        if (resultado != 0)
        {
            return resultado;
        }

        return x.OrdenDeLlegada.CompareTo(y.OrdenDeLlegada);
    }
}
