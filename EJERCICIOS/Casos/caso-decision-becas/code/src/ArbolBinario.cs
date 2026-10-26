namespace ClasificacionBecas;

/// <summary>
/// Guarda la raíz y ofrece operaciones estructurales. No conoce reglas ni solicitudes.
/// La altura se mide en aristas: una hoja tiene altura 0 y un árbol vacío, -1.
/// </summary>
/// <typeparam name="T">Tipo del dato que guardan los nodos.</typeparam>
public class ArbolBinario<T>
{
    /// <summary>
    /// Raíz del árbol; <c>null</c> si el árbol está vacío.
    /// </summary>
    public NodoBinario<T>? Raiz { get; private set; }

    /// <summary>
    /// Crea el árbol a partir de su raíz.
    /// </summary>
    /// <param name="raiz">Nodo raíz; <c>null</c> representa un árbol vacío.</param>
    public ArbolBinario(NodoBinario<T>? raiz)
    {
        Raiz = raiz;
    }

    /// <summary>
    /// Cuenta los nodos del árbol.
    /// </summary>
    /// <returns>Número de nodos; 0 si el árbol está vacío.</returns>
    public int ContarNodos()
    {
        return ContarNodos(Raiz);
    }

    /// <summary>
    /// Cuenta las hojas, es decir, los nodos sin hijos.
    /// </summary>
    /// <returns>Número de hojas.</returns>
    public int ContarHojas()
    {
        return ContarHojas(Raiz);
    }

    /// <summary>
    /// Calcula la altura del árbol, medida en aristas.
    /// </summary>
    /// <returns>Aristas del camino más largo desde la raíz hasta una hoja; -1 si el árbol está vacío.</returns>
    public int Altura()
    {
        return Altura(Raiz);
    }

    /// <summary>
    /// Indica si el árbol es lleno: cada nodo tiene 0 o 2 hijos.
    /// </summary>
    /// <returns><c>true</c> si ningún nodo tiene un solo hijo.</returns>
    public bool EsLleno()
    {
        return EsLleno(Raiz);
    }

    /// <summary>
    /// Recorre el árbol en preorden: nodo, izquierdo, derecho.
    /// </summary>
    /// <returns>Los nodos en el orden en que se visitan.</returns>
    public List<NodoBinario<T>> Preorden()
    {
        List<NodoBinario<T>> visitados = new List<NodoBinario<T>>();
        Preorden(Raiz, visitados);
        return visitados;
    }

    /// <summary>
    /// Recorre el árbol en inorden: izquierdo, nodo, derecho.
    /// </summary>
    /// <returns>Los nodos en el orden en que se visitan.</returns>
    public List<NodoBinario<T>> Inorden()
    {
        List<NodoBinario<T>> visitados = new List<NodoBinario<T>>();
        Inorden(Raiz, visitados);
        return visitados;
    }

    /// <summary>
    /// Recorre el árbol en postorden: izquierdo, derecho, nodo.
    /// </summary>
    /// <returns>Los nodos en el orden en que se visitan.</returns>
    public List<NodoBinario<T>> Postorden()
    {
        List<NodoBinario<T>> visitados = new List<NodoBinario<T>>();
        Postorden(Raiz, visitados);
        return visitados;
    }

    /// <summary>
    /// Recorre el árbol por niveles con una cola y agrupa los nodos de cada nivel.
    /// </summary>
    /// <returns>Una lista por nivel, desde la raíz (nivel 0) hacia abajo.</returns>
    public List<List<NodoBinario<T>>> Niveles()
    {
        List<List<NodoBinario<T>>> niveles = new List<List<NodoBinario<T>>>();
        if (Raiz == null)
        {
            return niveles;
        }

        Queue<NodoBinario<T>> pendientes = new Queue<NodoBinario<T>>();
        pendientes.Enqueue(Raiz);

        while (pendientes.Count > 0)
        {
            // Count al empezar el nivel es exactamente el número de nodos de ese nivel.
            int nodosDelNivel = pendientes.Count;
            List<NodoBinario<T>> nivel = new List<NodoBinario<T>>();

            for (int indice = 0; indice < nodosDelNivel; indice++)
            {
                NodoBinario<T> nodo = pendientes.Dequeue();
                nivel.Add(nodo);

                if (nodo.Izquierdo != null)
                {
                    pendientes.Enqueue(nodo.Izquierdo);
                }

                if (nodo.Derecho != null)
                {
                    pendientes.Enqueue(nodo.Derecho);
                }
            }

            niveles.Add(nivel);
        }

        return niveles;
    }

    /// <summary>
    /// Obtiene el camino desde la raíz hasta un nodo.
    /// </summary>
    /// <param name="objetivo">Nodo que se busca.</param>
    /// <returns>Los nodos del camino, empezando por la raíz; una lista vacía si el nodo no está en el árbol.</returns>
    public List<NodoBinario<T>> CaminoHacia(NodoBinario<T> objetivo)
    {
        List<NodoBinario<T>> camino = new List<NodoBinario<T>>();
        ExtenderCamino(Raiz, objetivo, camino);
        return camino;
    }

    /// <summary>
    /// Cuenta los nodos del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    private int ContarNodos(NodoBinario<T>? nodo)
    {
        if (nodo == null)
        {
            return 0;
        }

        return 1 + ContarNodos(nodo.Izquierdo) + ContarNodos(nodo.Derecho);
    }

    /// <summary>
    /// Cuenta las hojas del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    private int ContarHojas(NodoBinario<T>? nodo)
    {
        if (nodo == null)
        {
            return 0;
        }

        if (nodo.EsHoja)
        {
            return 1;
        }

        return ContarHojas(nodo.Izquierdo) + ContarHojas(nodo.Derecho);
    }

    /// <summary>
    /// Calcula la altura del subárbol que empieza en <paramref name="nodo"/>.
    /// </summary>
    /// <remarks>
    /// Es un recorrido en postorden: necesita la altura de los dos hijos antes de calcular la del nodo.
    /// </remarks>
    private int Altura(NodoBinario<T>? nodo)
    {
        if (nodo == null)
        {
            return -1;
        }

        int alturaIzquierda = Altura(nodo.Izquierdo);
        int alturaDerecha = Altura(nodo.Derecho);
        return 1 + Math.Max(alturaIzquierda, alturaDerecha);
    }

    /// <summary>
    /// Comprueba que ningún nodo del subárbol tenga un solo hijo.
    /// </summary>
    private bool EsLleno(NodoBinario<T>? nodo)
    {
        if (nodo == null)
        {
            return true;
        }

        bool tieneIzquierdo = nodo.Izquierdo != null;
        bool tieneDerecho = nodo.Derecho != null;
        if (tieneIzquierdo != tieneDerecho)
        {
            return false;
        }

        return EsLleno(nodo.Izquierdo) && EsLleno(nodo.Derecho);
    }

    /// <summary>
    /// Agrega a <paramref name="visitados"/> el subárbol de <paramref name="nodo"/> en preorden.
    /// </summary>
    private void Preorden(NodoBinario<T>? nodo, List<NodoBinario<T>> visitados)
    {
        if (nodo == null)
        {
            return;
        }

        visitados.Add(nodo);
        Preorden(nodo.Izquierdo, visitados);
        Preorden(nodo.Derecho, visitados);
    }

    /// <summary>
    /// Agrega a <paramref name="visitados"/> el subárbol de <paramref name="nodo"/> en inorden.
    /// </summary>
    private void Inorden(NodoBinario<T>? nodo, List<NodoBinario<T>> visitados)
    {
        if (nodo == null)
        {
            return;
        }

        Inorden(nodo.Izquierdo, visitados);
        visitados.Add(nodo);
        Inorden(nodo.Derecho, visitados);
    }

    /// <summary>
    /// Agrega a <paramref name="visitados"/> el subárbol de <paramref name="nodo"/> en postorden.
    /// </summary>
    private void Postorden(NodoBinario<T>? nodo, List<NodoBinario<T>> visitados)
    {
        if (nodo == null)
        {
            return;
        }

        Postorden(nodo.Izquierdo, visitados);
        Postorden(nodo.Derecho, visitados);
        visitados.Add(nodo);
    }

    /// <summary>
    /// Busca <paramref name="objetivo"/> en profundidad y deja en <paramref name="camino"/> el camino hasta él.
    /// </summary>
    /// <remarks>
    /// Recorrido con retroceso: el nodo entra a la ruta al bajar y sale si ninguna de sus ramas contiene el objetivo.
    /// </remarks>
    /// <returns><c>true</c> si el objetivo está en este subárbol.</returns>
    private bool ExtenderCamino(NodoBinario<T>? nodo, NodoBinario<T> objetivo, List<NodoBinario<T>> camino)
    {
        if (nodo == null)
        {
            return false;
        }

        camino.Add(nodo);
        if (nodo == objetivo)
        {
            return true;
        }

        if (ExtenderCamino(nodo.Izquierdo, objetivo, camino) || ExtenderCamino(nodo.Derecho, objetivo, camino))
        {
            return true;
        }

        camino.RemoveAt(camino.Count - 1);
        return false;
    }
}
