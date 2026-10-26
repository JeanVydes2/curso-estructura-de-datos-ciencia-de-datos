using ClasificacionBecas.Modelos;

namespace ClasificacionBecas;

/// <summary>
/// Construye el árbol a partir de la tabla de reglas.
/// Convención del comité: la respuesta "Sí" va a Izquierdo y la respuesta "No" va a Derecho.
/// </summary>
public static class ConstructorDeArbol
{
    /// <summary>
    /// Construye el árbol de decisión a partir de la tabla de reglas.
    /// </summary>
    /// <remarks>
    /// Crea un nodo por fila, conecta Sí con <c>Izquierdo</c> y No con <c>Derecho</c>, busca la raíz y
    /// comprueba que todas las reglas se alcancen desde ella. Lanza <c>InvalidOperationException</c>
    /// si hay ids repetidos, salidas inexistentes, una regla con dos padres, más de una raíz o un ciclo.
    /// </remarks>
    /// <param name="reglas">Filas de la tabla, en cualquier orden.</param>
    /// <returns>El árbol con raíz en la única regla que no es salida de otra.</returns>
    public static ArbolBinario<Regla> Construir(List<Regla> reglas)
    {
        // 1. Un nodo por fila, indexado por Id.
        Dictionary<string, NodoBinario<Regla>> nodos = new Dictionary<string, NodoBinario<Regla>>();
        foreach (Regla regla in reglas)
        {
            if (nodos.ContainsKey(regla.Id))
            {
                throw new InvalidOperationException("La regla " + regla.Id + " está repetida.");
            }

            nodos[regla.Id] = new NodoBinario<Regla>(regla);
        }

        // 2. Conectar las preguntas con sus dos salidas y registrar quién tiene padre.
        HashSet<string> conPadre = new HashSet<string>();
        foreach (Regla regla in reglas)
        {
            if (!regla.EsPregunta)
            {
                continue;
            }

            NodoBinario<Regla> nodo = nodos[regla.Id];
            nodo.Izquierdo = BuscarHijo(nodos, regla.Id, regla.IdSi, conPadre);
            nodo.Derecho = BuscarHijo(nodos, regla.Id, regla.IdNo, conPadre);
        }

        // 3. La raíz es la única regla que no aparece como salida de otra.
        List<string> raices = new List<string>();
        foreach (Regla regla in reglas)
        {
            if (!conPadre.Contains(regla.Id))
            {
                raices.Add(regla.Id);
            }
        }

        if (raices.Count != 1)
        {
            throw new InvalidOperationException("La tabla debe tener exactamente una raíz y tiene " + raices.Count + ".");
        }

        ArbolBinario<Regla> arbol = new ArbolBinario<Regla>(nodos[raices[0]]);

        // 4. Con una raíz y un padre por nodo, un ciclo dejaría nodos fuera del alcance de la raíz.
        if (arbol.ContarNodos() != reglas.Count)
        {
            throw new InvalidOperationException("Hay reglas que no se alcanzan desde la raíz: la tabla tiene un ciclo.");
        }

        return arbol;
    }

    /// <summary>
    /// Busca una regla por su identificador recorriendo el árbol en preorden.
    /// </summary>
    /// <param name="arbol">Árbol de reglas.</param>
    /// <param name="id">Identificador, por ejemplo D5.</param>
    /// <returns>El nodo de la regla, o <c>null</c> si no existe.</returns>
    public static NodoBinario<Regla>? BuscarPorId(ArbolBinario<Regla> arbol, string id)
    {
        foreach (NodoBinario<Regla> nodo in arbol.Preorden())
        {
            if (nodo.Valor.Id == id)
            {
                return nodo;
            }
        }

        return null;
    }

    /// <summary>
    /// Devuelve el nodo al que apunta una salida y registra que ese nodo ya tiene padre.
    /// </summary>
    /// <remarks>
    /// Lanza <c>InvalidOperationException</c> si la salida no existe o si el nodo ya tenía padre.
    /// </remarks>
    private static NodoBinario<Regla> BuscarHijo(Dictionary<string, NodoBinario<Regla>> nodos,
        string idPadre, string idHijo, HashSet<string> conPadre)
    {
        if (!nodos.ContainsKey(idHijo))
        {
            throw new InvalidOperationException("La regla " + idPadre + " apunta a " + idHijo + ", que no existe.");
        }

        if (!conPadre.Add(idHijo))
        {
            throw new InvalidOperationException("La regla " + idHijo + " tiene dos padres.");
        }

        return nodos[idHijo];
    }
}
