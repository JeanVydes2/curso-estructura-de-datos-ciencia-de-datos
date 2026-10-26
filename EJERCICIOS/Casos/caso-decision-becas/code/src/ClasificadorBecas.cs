using ClasificacionBecas.Modelos;

namespace ClasificacionBecas;

/// <summary>
/// Clasifica una solicitud bajando desde la raíz: una decisión por nivel, hasta llegar a un apoyo.
/// Visita a lo sumo altura + 1 nodos.
/// </summary>
public static class ClasificadorBecas
{
    /// <summary>
    /// Clasifica una solicitud: desde la raíz responde una pregunta por nivel hasta llegar a un apoyo.
    /// </summary>
    /// <param name="arbol">Árbol de reglas.</param>
    /// <param name="solicitud">Solicitud con sus datos.</param>
    /// <returns>Los nodos visitados; el último es el apoyo. Son a lo sumo altura + 1 nodos.</returns>
    public static List<NodoBinario<Regla>> Clasificar(ArbolBinario<Regla> arbol, Solicitud solicitud)
    {
        List<NodoBinario<Regla>> camino = new List<NodoBinario<Regla>>();
        NodoBinario<Regla>? actual = arbol.Raiz;

        while (actual != null)
        {
            camino.Add(actual);
            if (!actual.Valor.EsPregunta)
            {
                break;
            }

            if (solicitud.Responde(actual.Valor.Chequeo))
            {
                actual = actual.Izquierdo;
            }
            else
            {
                actual = actual.Derecho;
            }
        }

        return camino;
    }
}
