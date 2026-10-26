using LluviaTopK.Modelos;

namespace LluviaTopK;

/// <summary>
/// Obtiene los K puntos con más lluvia (o con menos) de un lote de lecturas,
/// recorriéndolo una sola vez con un montículo de tamaño K.
/// </summary>
public static class SeleccionTopK
{
    /// <summary>
    /// Obtiene los K acumulados mayores de un lote, en orden de mayor a menor.
    /// </summary>
    /// <param name="lecturas">Lote de lecturas, en el orden en que llegan.</param>
    /// <param name="k">Cantidad de puntos que se piden.</param>
    /// <returns>Los K mayores, de mayor a menor; menos de K si el lote es más corto.</returns>
    public static List<Acumulado> TopK(IEnumerable<Acumulado> lecturas, int k)
    {
        return Seleccionar(lecturas, k, false);
    }

    /// <summary>
    /// Obtiene los K acumulados menores de un lote, en orden de menor a mayor.
    /// </summary>
    /// <param name="lecturas">Lote de lecturas, en el orden en que llegan.</param>
    /// <param name="k">Cantidad de puntos que se piden.</param>
    /// <returns>Los K menores, de menor a mayor; menos de K si el lote es más corto.</returns>
    public static List<Acumulado> BottomK(IEnumerable<Acumulado> lecturas, int k)
    {
        return Seleccionar(lecturas, k, true);
    }

    /// <summary>
    /// Recorre el lote manteniendo un montículo de tamaño K.
    /// </summary>
    /// <remarks>
    /// Para los K mayores se usa un montículo de mínimos: la raíz es el menor de los K y
    /// es el que sale cuando llega una lectura mayor. Para los K menores se invierte el
    /// comparador, y el recorrido es el mismo.
    /// </remarks>
    private static List<Acumulado> Seleccionar(IEnumerable<Acumulado> lecturas, int k, bool menores)
    {
        List<Acumulado> resultado = new List<Acumulado>();
        if (k <= 0)
        {
            return resultado;
        }

        ComparadorPorMilimetros baseComparer = new ComparadorPorMilimetros();
        IComparer<Acumulado> comparador = menores ? new ComparadorInvertido(baseComparer) : baseComparer;
        MonticuloBinario<Acumulado> monticulo = new MonticuloBinario<Acumulado>(comparador);

        foreach (Acumulado lectura in lecturas)
        {
            if (monticulo.Cantidad < k)
            {
                monticulo.Insertar(lectura);
            }
            else if (comparador.Compare(lectura, monticulo.VerRaiz()) > 0)
            {
                monticulo.ExtraerRaiz();
                monticulo.Insertar(lectura);
            }
        }

        while (!monticulo.EstaVacio)
        {
            resultado.Add(monticulo.ExtraerRaiz());
        }

        // El montículo entrega los K en el orden de su comparador; se ordena el resultado.
        if (menores)
        {
            resultado.Sort(baseComparer);
        }
        else
        {
            resultado.Sort((x, y) => baseComparer.Compare(y, x));
        }

        return resultado;
    }

    /// <summary>
    /// Invierte el orden de un comparador, para pasar de los K mayores a los K menores.
    /// </summary>
    private class ComparadorInvertido : IComparer<Acumulado>
    {
        private readonly IComparer<Acumulado> interno;

        /// <summary>
        /// Crea un comparador que invierte el sentido de otro.
        /// </summary>
        /// <param name="interno">Comparador que se invierte.</param>
        public ComparadorInvertido(IComparer<Acumulado> interno)
        {
            this.interno = interno;
        }

        /// <summary>
        /// Compara dos acumulados en sentido contrario al comparador interno.
        /// </summary>
        /// <param name="x">Primer acumulado.</param>
        /// <param name="y">Segundo acumulado.</param>
        /// <returns>El resultado del comparador interno, con el signo invertido.</returns>
        public int Compare(Acumulado? x, Acumulado? y)
        {
            return -interno.Compare(x, y);
        }
    }
}
